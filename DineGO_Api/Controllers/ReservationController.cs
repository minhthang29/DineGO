using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Models;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Controller for managing reservation operations such as creating, updating, deleting, and retrieving reservations.
    /// </summary>
    /// <author>Thanhdt, Thangtm, Sieuhdd</author>
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITableRepository _tableRepository;

        /// <summary>
        /// Constructor that injects the reservation repository for handling reservation data.
        /// </summary>
        public ReservationController(IReservationRepository reservationRepository, ITableRepository tableRepository)
        {
            _reservationRepository = reservationRepository;
            _tableRepository = tableRepository;
        }

        /// <summary>
        /// Retrieves the list of all reservations.
        /// </summary>
        /// <returns>List of reservations.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_reservationRepository.GetReservations());
        }

        /// <summary>
        /// Retrieves a specific reservation by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation.</param>
        /// <returns>Reservation data or 404 if not found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var reservation = _reservationRepository.FindReservationById(id);
            if (reservation == null)
                return NotFound(string.Format(NotificationConstants.RESERVATION_WITH_ID_NOT_FOUND, id));
            return Ok(reservation);
        }

        /// <summary>
        /// Creates a new reservation.
        /// </summary>
        /// <param name="reservation">The reservation object to be created.</param>
        /// <returns>201 Created with the created reservation.</returns>
        [HttpPost]
        public IActionResult AddReservation(Reservation reservation)
        {
            _reservationRepository.SaveReservation(reservation);
            return Ok(reservation);
        }

        /// <summary>
        /// Updates an existing reservation.
        /// </summary>
        /// <param name="id">ID of the reservation to update.</param>
        /// <param name="reservation">Updated reservation data.</param>
        /// <returns>200 OK with updated list or 400 if ID mismatch.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.reser_id)
                return BadRequest(NotificationConstants.RESERVATION_ID_MISMATCH);

            _reservationRepository.UpdateReservation(reservation);
            return Ok(_reservationRepository.GetReservations());
        }

        /// <summary>
        /// Deletes a reservation by its ID.
        /// </summary>
        /// <param name="id">ID of the reservation to delete.</param>
        /// <returns>204 No Content if successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            _reservationRepository.DeleteReservation(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all reservations made by a specific customer, including restaurant info.
        /// </summary>
        /// <param name="cus_id">The customer ID.</param>
        /// <returns>List of reservations for the given customer.</returns>
        [HttpGet("cus_id")]
        public IActionResult GetReservationsWithRestaurantName(int cus_id)
        {
            var reservations = _reservationRepository.GetResByCusId(cus_id);
            return Ok(reservations);
        }

        /// <summary>
        /// Retrieves all reservations for a specific restaurant.
        /// </summary>
        /// <param name="res_id">The restaurant ID.</param>
        /// <returns>List of reservations for the specified restaurant.</returns>
        [HttpGet("res_id")]
        public IActionResult GetReservationByRestaurant(int res_id)
        {
            var reservations = _reservationRepository.GetResByResId(res_id);
            return Ok(reservations ?? new List<Reservation>());
        }

        [HttpPut("status/{id}")]
        public IActionResult UpdateReservationStatus(int id, [FromQuery] int reser_status)
        {
            var reservation = _reservationRepository.FindReservationById(id);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đơn đặt bàn." });

            reservation.reser_status = reser_status;
            _reservationRepository.UpdateReservation(reservation);

            return Ok(new { message = $"Đã cập nhật trạng thái: {reser_status} cho đơn và bàn." });
        }

        [HttpGet("GetReservationByTable")]
        public IActionResult GetReservationByTable(int table_id)
        {
            var reservations = _reservationRepository.GetReservationsByTable(table_id);
            return Ok(reservations);
        }

        [HttpGet("GetAvailableSlots")]
        public IActionResult GetAvailableSlots(int table_id, DateTime date)
        {
            // 1. Lấy table và restaurant
            var table = _tableRepository.FindTableById(table_id);
            if (table == null || table.res_id == 0)
                return NotFound(new { message = "Không tìm thấy bàn hoặc nhà hàng." });

            var restaurant = table.Restaurant;
            if (restaurant == null)
                return BadRequest(new { message = "ko thấy" });
            if (restaurant.res_open_time == null || restaurant.res_close_time == null)
                return BadRequest(new { message = "Nhà hàng chưa cấu hình giờ mở cửa/đóng cửa." });

            var openTime = restaurant.res_open_time.Value;   // TimeSpan
            var closeTime = restaurant.res_close_time.Value; // TimeSpan

            // 2. Tạo slot theo giờ mở/đóng (ví dụ: mỗi 30 phút)
            var slots = new List<TimeSpan>();
            for (var t = openTime; t <= closeTime - TimeSpan.FromHours(2); t = t.Add(TimeSpan.FromMinutes(30)))
            {
                slots.Add(t);
            }

            // 3. Lấy reservation trong ngày cho bàn
            var reservations = _reservationRepository.GetReservationsByTable(table_id)
                .Where(r => r.reser_date.Date == date.Date && r.reser_status != 2 && r.reser_status != 4 && r.reser_status != 5) // bỏ đơn bị hủy, hoàn thành, không có mặt
                .ToList();

            // 4. Loại bỏ các slot trùng với khoảng ±2h của mỗi reservation
            foreach (var reservation in reservations)
            {
                var reservedTime = reservation.reser_date.TimeOfDay;
                var blockStart = reservedTime - TimeSpan.FromHours(2);
                var blockEnd = reservedTime + TimeSpan.FromHours(2);

                slots = slots
                    .Where(s => s <= blockStart || s >= blockEnd)
                    .ToList();
            }

            // 4.5. Nếu ngày là hôm nay, chỉ lấy slot >= (giờ hiện tại + 30p)
            if (date.Date == DateTime.Now.Date)
            {
                var nowPlus1h = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(0.5));
                slots = slots.Where(s => s >= nowPlus1h).ToList();
            }

            // 5. Trả về dạng string
            var available = slots.Select(s => $"{s:hh\\:mm}").ToList();
            return Ok(available);
        }

        [HttpGet("GetAllSlots")]
        public IActionResult GetAllSlots(int table_id, DateTime date)
        {
            var table = _tableRepository.FindTableById(table_id);
            if (table == null || table.res_id == 0)
                return NotFound(new { message = "Không tìm thấy bàn hoặc nhà hàng." });

            var restaurant = table.Restaurant;
            if (restaurant == null)
                return BadRequest(new { message = "Không thấy nhà hàng." });
            if (restaurant.res_open_time == null || restaurant.res_close_time == null)
                return BadRequest(new { message = "Nhà hàng chưa cấu hình giờ mở/đóng cửa." });

            var openTime = restaurant.res_open_time.Value;
            var closeTime = restaurant.res_close_time.Value;

            // Tạo slot 30 phút
            var slots = new List<TimeSpan>();
            for (var t = openTime; t <= closeTime - TimeSpan.FromHours(2); t = t.Add(TimeSpan.FromMinutes(30)))
            {
                slots.Add(t);
            }

            // Lấy reservation trong ngày
            var reservations = _reservationRepository.GetReservationsByTable(table_id)
    .Where(r => r.reser_date.Date == date.Date)
    .ToList();

            // Map status code sang text
            string MapStatus(int status)
            {
                return status switch
                {
                    0 => "pending",
                    1 => "accepted",
                    2 => "canceled",
                    3 => "show",
                    4 => "no-show",
                    5 => "completed",
                    _ => "reserved"
                };
            }

            // Đánh dấu slot
            var result = slots.Select(s =>
{
    Reservation reservation = null;
    string status = "available";

    foreach (var r in reservations)
    {
        var reservedTime = r.reser_date.TimeOfDay;
        var reservedEnd = reservedTime + TimeSpan.FromMinutes(30);

        // 1️⃣ Slot trùng reservation → ưu tiên
        if (s >= reservedTime && s < reservedEnd)
        {
            status = MapStatus(r.reser_status);
            reservation = r;
            break;
        }

        // 2️⃣ Slot khác trong ±2h → blocked
        var blockStart = reservedTime - TimeSpan.FromHours(2);
        var blockEnd = reservedTime + TimeSpan.FromHours(2);

        if (s > blockStart && s < blockEnd)
        {
            status = "blocked";
            break;
        }
    }

    // 3️⃣ Nếu là hôm nay → disable slot đã qua (chỉ khi chưa accepted/show)
    if (date.Date == DateTime.Now.Date)
    {
        var now = DateTime.Now.TimeOfDay;
        if (s < now && status != "accepted" && status != "show")
            status = "blocked";
    }

    return new
    {
        Time = $"{s:hh\\:mm}",
        Status = status,
        ReserId = reservation?.reser_id
    };
});
            return Ok(result);
        }
    }
}
