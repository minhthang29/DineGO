using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using DineGO_Client.SignalR;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles reservation-related actions such as creating and viewing reservations.
    /// </summary>
    /// <author>Sieuhdd;Thangtm</author>
    [Route("[controller]")]
    public class ReservationController : Controller
    {
        private readonly ILogger<Reservation> _logger;
        private readonly ApiService _apiService;
        private readonly TableService _tableService;
        private readonly IHubContext<ReservationHub> _hubContext;
        private readonly CustomerPointService _pointService;


        public ReservationController(
     ILogger<Reservation> logger,
     ApiService apiService,
     TableService tableService,
     IHubContext<ReservationHub> hubContext,
     CustomerPointService customerPointService)
        {
            _logger = logger;
            _apiService = apiService;
            _tableService = tableService;
            _hubContext = hubContext;
            _pointService = customerPointService;
        }


        /// <summary>
        /// Displays the reservation page for a restaurant.
        /// </summary>
        /// <param name="id">The restaurant ID.</param>
        /// <returns>Returns the view with restaurant and customer details.</returns>
        /// <author>Thangtm</author>

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{id}");
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);

            if (cus_id == null)
                throw new UnauthorizedAccessException();

            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");
            var areas = await _apiService.GetAsync<List<TableArea>>($"{ApiEndpoints.AREA_BY_RESID}{id}");
            var tables = await _apiService.GetAsync<List<Table>>(ApiEndpoints.TABLE);
            tables = tables.Where(t => t.res_id == id && !t.table_is_deleted).ToList();

            var viewModel = new CustomReservationViewModel
            {
                Restaurant = restaurant,
                Customer = customer,
                Areas = areas,
                Tables = tables
            };

            return View(viewModel);
        }

        /// <summary>
        /// Creates a reservation for a customer at a specific restaurant.
        /// </summary>
        /// <param name="model">The reservation model containing the details.</param>
        /// <param name="reser_date_date">The date of the reservation.</param>
        /// <param name="reser_date_time">The time of the reservation.</param>
        /// <returns>Redirects to the reservation page or returns an error message if invalid.</returns>
        /// <author>ThangTM</author>

        [HttpPost("CreateReservation")]
        public async Task<IActionResult> CreateReservation(
    [FromForm] Reservation model,
    [FromForm] string reser_date_date,
    [FromForm] string reser_date_time,
    [FromForm] bool isAdminMode = false,  // ← Param đơn giản để trigger admin (từ JS)
    [FromForm] string? adminNote = null)   // ← Note tùy chọn cho admin
        {
            bool isAdmin = isAdminMode;  // Detect admin mode

            int? customerId = null;
            string finalNote = model.reser_note ?? "";

            if (isAdmin)
            {
                // Admin mode: cus_id = null, status=1, note = "Admin đặt" + adminNote
                customerId = null;
                finalNote = string.IsNullOrEmpty(adminNote) ? "Nhà hàng đặt" : $"Nhà hàng đặt - {adminNote}";
                model.res_id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID).Value;
            }
            else
            {
                // Mode thường (customer login): Giữ nguyên code cũ
                customerId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
                if (!customerId.HasValue || customerId == 0)
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.YOU_NOT_LOGIN;
                    return RedirectToAction("Index", new { id = model.res_id });
                }

                int cusId = customerId.Value;

                // Load thông tin customer
                var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cusId}");
                if (customer == null)
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = "Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Index", new { id = model.res_id });
                }

                // Kiểm tra thiếu phone/email
                if (string.IsNullOrWhiteSpace(customer.cus_phone) || string.IsNullOrWhiteSpace(customer.cus_email))
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = "Vui lòng cập nhật số điện thoại và email trước khi đặt bàn.";
                    return RedirectToAction("Profile", "Customer"); // 👈 chuyển hướng sang trang hồ sơ
                }

                // Check pending reservations (giữ nguyên)
                var customerReservations = await _apiService.GetAsync<List<Reservation>>(
                    $"{ApiEndpoints.RESERVATION_BY_CUSID}{cusId}");

                if (customerReservations.Any(r => r.reser_status == 0))
                {
                    TempData[KeyConstants.ERROR_MESSAGE] =
                        "Bạn đang có một đơn đặt bàn chưa hoàn tất thanh toán. Vui lòng hoàn tất hoặc hủy đơn trước khi đặt thêm.";
                    return RedirectToAction("Index", new { id = model.res_id });
                }
            }

            // ✅ THÊM: Validate model cơ bản trước parse date (tránh exception)
            if (model.table_id <= 0)
            {
                string errMsg = "ID bàn không hợp lệ.";
                if (isAdmin) return Json(new { success = false, message = errMsg });
                else { TempData[KeyConstants.ERROR_MESSAGE] = errMsg; return RedirectToAction("Index", new { id = model.res_id }); }
            }


            // Logic chung (tái sử dụng: parse date, check conflict)
            string dateTimeString = $"{reser_date_date} {reser_date_time}";

            if (!DateTime.TryParseExact(
                dateTimeString,
                new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime reservationDate))
            {
                if (isAdmin)
                    return Json(new { success = false, message = NotificationConstants.DATE_NOT_YYYY_MM_DD_HH_MM });
                else
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATE_NOT_YYYY_MM_DD_HH_MM;
                    return RedirectToAction("Index", new { id = model.res_id });
                }
            }

            var existed = await _apiService.GetAsync<List<Reservation>>(
                $"{ApiEndpoints.RESERVATION_BY_TABLEID}{model.table_id}");

            if (existed.Any(r =>
                r.reser_status != 2 &&
                Math.Abs((r.reser_date - reservationDate).TotalHours) < 2))
            {
                string errorMsg = "Bàn này đã được đặt trong khung giờ gần với thời điểm bạn chọn (±2h). Vui lòng chọn giờ khác.";
                if (isAdmin)
                    return Json(new { success = false, message = errorMsg });
                else
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = errorMsg;
                    return RedirectToAction("Index", new { id = model.res_id });
                }
            }

            var reservation = new Reservation
            {
                cus_id = customerId,  // Null cho admin
                res_id = model.res_id,
                table_id = model.table_id,
                reser_date = reservationDate,
                reser_status = isAdmin ? 1 : 0,  // Admin: 1 (Đã đặt), thường: 0
                reser_note = finalNote,
                reser_create_at = DateTime.Now,
                reser_is_deleted = false
            };

            var created = await _apiService.PostAsync<Reservation, Reservation>(
                $"{ApiEndpoints.RESERVATION}", reservation);

            if (created != null)
            {
                await _hubContext.Clients.All.SendAsync("ReservationUpdated", model.table_id, reservationDate);

                if (isAdmin)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Tạo đặt bàn thành công!",
                        reser_id = created.reser_id
                    });
                }
                else
                {
                    return RedirectToAction("Payment", new { reser_id = created.reser_id });
                }
            }

            string failMsg = NotificationConstants.RESERVATION_FAILED;
            if (isAdmin)
                return Json(new { success = false, message = failMsg });
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = failMsg;
                return RedirectToAction("Index", new { id = model.res_id });
            }
        }


        /// <summary>
        /// Displays the payment page.
        /// </summary>
        /// <returns>Returns the view for payment.</returns>
        /// <author>ThangTM</author>
        [HttpGet("Payment")]
        public async Task<IActionResult> Payment(int reser_id)
        {
            var reservation = await _apiService.GetAsync<Reservation>(
               $"{ApiEndpoints.RESERVATION}/{reser_id}"
           );
            return View(reservation);
        }


        [HttpPut("ConfirmReservation")]
        public async Task<IActionResult> ConfirmReservation(int reser_id)
        {
            var reservation = await _apiService.GetAsync<Reservation>(
              $"{ApiEndpoints.RESERVATION}/{reser_id}"
          );

            await _tableService.UpdateReservationStatus(reser_id, 1);
            await _hubContext.Clients.All.SendAsync("ReceiveTableStatus", reservation.table_id, 2);
            return Ok();
        }

        [HttpPut("CancelIfExpired")]
        public async Task<IActionResult> CancelIfExpired(int reser_id)
        {
            var reservation = await _apiService.GetAsync<Reservation>($"{ApiEndpoints.RESERVATION}/{reser_id}");

            var diff = DateTime.Now - reservation.reser_create_at;
            if (diff.TotalMinutes < 10)
                return Ok(new { success = false, message = "Đơn chưa hết hạn." });

            await _tableService.UpdateReservationStatus(reser_id, 2);

            return Ok(new { success = true, message = "Đã hủy đơn quá hạn." });
        }

        [HttpGet("GetReservationInfo/{reserId}")]
        public async Task<IActionResult> GetReservationInfo(int reserId)
        {
            var reservation = await _apiService.GetAsync<Reservation>(
                $"{ApiEndpoints.RESERVATION}/{reserId}"
            );

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpPut("UpdateReserStatus")]
        public async Task<IActionResult> UpdateReserStatus(int reser_id, int reser_status)
        {
            try
            {
                var reservation = await _apiService.GetAsync<Reservation>(
                $"{ApiEndpoints.RESERVATION}/{reser_id}"
            );
                await _tableService.UpdateReservationStatus(reser_id, reser_status);
                // ✅ Nếu có bất kỳ cập nhật trạng thái bàn push realtime
                await _hubContext.Clients.All.SendAsync(
                    "ReservationUpdated",
                    reservation.table_id,
                    reservation.reser_date
                );
                return Ok(new { success = true, message = "Cập nhật trạng thái reservation thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("CheckPaid")]
        public async Task<IActionResult> CheckPaid(int reser_id)
        {
            try
            {
                var reservation = await _apiService.GetAsync<Reservation>(
                    $"{ApiEndpoints.RESERVATION}/{reser_id}"
                );
                if (reservation == null)
                    return NotFound(new { success = false, message = "Không tìm thấy đơn." });

                // Lấy raw object bằng JsonDocument
                var jsonDoc = await _apiService.GetAsync<JsonDocument>(
                    $"{ApiEndpoints.PAYMENT}/GetTransactions"
                );

                // Log dữ liệu raw JSON trả về từ Casso
                var rawJson = jsonDoc.RootElement.GetRawText();
                // Convert sang JObject để xử lý bằng Newtonsoft
                var obj = JObject.Parse(rawJson);
                var records = obj["data"]?["records"] as JArray;

                if (records == null)
                    return Ok(new { success = false, message = "Không có giao dịch nào." });

                DateTime reservationTime = reservation.reser_create_at;

                foreach (var tx in records)
                {
                    var description = (string?)tx["description"] ?? "";
                    var amount = (decimal?)tx["amount"] ?? 0;
                    var paidAtStr = (string?)tx["when"] ?? "";

                    if (!DateTime.TryParse(paidAtStr, null, DateTimeStyles.AdjustToUniversal, out var paidAt))
                        continue;

                    var reservationTimeUtc = DateTime.SpecifyKind(reservation.reser_create_at, DateTimeKind.Local).ToUniversalTime();

                    if (description.Contains(reser_id.ToString()) &&
                        amount == reservation.restaurant.res_reservation_fee &&
                        paidAt.ToUniversalTime() >= reservationTimeUtc)
                    {
                        await _tableService.UpdateReservationStatus(reser_id, 1);
                        // await _hubContext.Clients.All.SendAsync("ReceiveTableStatus", reservation.table_id, 2);

                        // ✅ Tính và cộng điểm cho khách hàng
                        var depositAmount = reservation.restaurant.res_reservation_fee;
                        var rawPoints = (int)(depositAmount * 0.01m); // 100k = 1000 điểm
                        var bonusPoints = (int)(rawPoints * 0.01m);    // chỉ lấy 1%

                        await _pointService.UpdatePointsAsync(new CustomerPointRequest
                        {
                            CusId = reservation.cus_id.Value,
                            ChangeAmount = bonusPoints,
                            Description = $"Thanh toán đặt bàn thành công +{bonusPoints} điểm"
                        });

                        return Ok(new { success = true, message = $"Thanh toán thành công. Bạn đã được cộng {bonusPoints} điểm." });
                    }
                }

                return Ok(new { success = false, message = "Chưa tìm thấy giao dịch phù hợp." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi CheckPaid: {ex}");
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpGet("GetAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(int tableId, DateTime date)
        {
            try
            {
                var slots = await _apiService.GetAsync<List<string>>(
                    $"{ApiEndpoints.RESERVATION}/GetAvailableSlots?table_id={tableId}&date={date:yyyy-MM-dd}"
                );

                return Ok(slots);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi GetAvailableSlots: {ex}");
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        [HttpGet("GetAllSlots")]
        public async Task<IActionResult> GetAllSlots(int tableId, DateTime date)
        {
            try
            {
                var slots = await _apiService.GetAsync<List<object>>(
                    $"{ApiEndpoints.RESERVATION}/GetAllSlots?table_id={tableId}&date={date:yyyy-MM-dd}"
                );

                if (slots == null || !slots.Any())
                    return Ok(new List<string>()); // trả về mảng rỗng

                return Ok(slots);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi GetAvailableSlots: {ex}");
                return StatusCode(500, new { success = false, message = "Lỗi server", detail = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách giờ đã đặt hôm nay của 1 nhà hàng
        /// </summary>
        [HttpGet("GetReservedTimesToday")]
        public async Task<IActionResult> GetReservedTimesToday()
        {
            try
            {
                int resId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID) ?? 0;
                // gọi API trung tâm đã có sẵn
                var reservations = await _apiService.GetAsync<List<Reservation>>(
                    $"{ApiEndpoints.RESERVATION_BY_RESID}{resId}"
                );

                if (reservations == null || reservations.Count == 0)
                    return Ok(new List<object>());

                var today = DateTime.Today;

                var todayReservations = reservations
                    .Where(r => r.reser_date.Date == today && r.reser_status != 2) // bỏ canceled
                    .Select(r => new
                    {
                        r.reser_id,
                        r.table.table_name,
                        time = r.reser_date.ToString("HH:mm"),
                        r.reser_status
                    })
                    .OrderBy(r => r.time)
                    .ToList();

                return Ok(todayReservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }
    }
}