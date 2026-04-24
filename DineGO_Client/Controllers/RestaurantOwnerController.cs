using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Models.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Core.Models.Client.Custom;
using Core.Models.Client.RestaurantOwnerModel;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles actions related to restaurant owners, such as viewing and updating restaurant profile,
    /// managing reservations, and creating new restaurant owners.
    /// </summary>
    /// <author>Thangtm;Phuonghh</author>
    [Route("[controller]")]
    public class RestaurantOwnerController : Controller
    {
        private readonly ILogger<RestaurantOwnerController> _logger;
        private readonly ApiService _apiService;
        private readonly RestaurantService _restaurantService;
        private readonly RestaurantOwnerService _restaurantOwnerService;
        private readonly AdService _adService;
        private readonly ImageHelper _imageHelper;

        public RestaurantOwnerController(ILogger<RestaurantOwnerController> logger, ApiService apiService, RestaurantService restaurantService, RestaurantOwnerService restaurantOwnerService, AdService adservice, ImageHelper imageHelper)
        {
            _logger = logger;
            _apiService = apiService;
            _restaurantService = restaurantService;
            _restaurantOwnerService = restaurantOwnerService;
            _adService = adservice;
            _imageHelper = imageHelper;
        }

        /// <summary>
        /// Displays the restaurant profile, including confirmed, rejected, and pending reservations.
        /// </summary>
        /// <param name="id">The restaurant ID.</param>
        /// <returns>Returns the view with the restaurant profile and reservation information.</returns>
        /// <author>Thangtm</author>
        [HttpGet] // Explicit route
        public async Task<IActionResult> ViewReservation(int id)
        {
            var reservation = await _apiService.GetAsync<List<Reservation>>($"{ApiEndpoints.RESERVATION_BY_RESID}{id}");

            var confirmedOrRejectedReservations = reservation
                .Where(r => r.reser_status == 1 || r.reser_status == 2)
                .ToList();

            var pendingReservations = reservation
                .Where(r => r.reser_status == 0)
                .ToList();
            var viewModel = new CustomProfileViewModel
            {
                ConfirmedOrRejectedReservations = confirmedOrRejectedReservations,
                PendingReservations = pendingReservations
            };

            var response = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT}/{id}");

            ViewBag.res_id = response.res_id;
            ViewBag.res_name = response.res_name;
            ViewBag.res_address = response.res_address;
            ViewBag.res_phone = response.res_phone;
            ViewBag.res_description = response.res_description;
            ViewBag.res_rate = response.res_rate;
            ViewBag.res_reservation_fee = response.res_reservation_fee;
            ViewBag.res_discount_promotion = response.res_discount_promotion;
            ViewBag.res_images = response.res_images;
            ViewBag.cate_id = response.cate_id;
            ViewBag.res_owner_id = response.res_owner_id;

            return View(viewModel);
        }

        /// <summary>
        /// Updates the status of a reservation.
        /// </summary>
        /// <param name="reservation">The reservation object containing updated status.</param>
        /// <returns>Redirects to the restaurant profile after updating the reservation status.</returns>
        /// <author>Thangtm</author>
        [HttpPost("UpdateReservationStatus")] // Unique route
        public async Task<IActionResult> UpdateReservationStatus(Reservation reservation)
        {
            var updateData = new
            {
                reser_id = reservation.reser_id,
                cus_id = reservation.cus_id,
                res_id = reservation.res_id,
                reser_date = reservation.reser_date,
                reser_status = reservation.reser_status
            };

            System.Console.WriteLine(updateData.ToString());
            var response = await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.RESERVATION}/{reservation.reser_id}", updateData);

            return RedirectToAction(ControllerConstants.PROFILE_RESTAURANT, ControllerConstants.RESTAURANT_OWNER, new { id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID) });
        }

        [HttpGet("UpdateProfileRestaurant/{id}")]
        public async Task<IActionResult> UpdateProfileRestaurant(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByID(id);
            ViewBag.Restaurant = restaurant;
            if (restaurant == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Không tìm thấy nhà hàng.";
                return RedirectToAction("ProfileRestaurant");
            }
            return View("UpdateProfileRestaurant", restaurant);
        }

        /// <summary>
        /// Updates the restaurant profile information.
        /// </summary>
        /// <param name="restaurant">The restaurant object containing updated profile data.</param>
        /// <returns>Redirects to the restaurant profile after updating the information.</returns>
        /// <author>Thangtm</author>
        [HttpPost("UpdateProfileRestaurant/{id}")]
        public async Task<IActionResult> UpdateProfileRestaurant(int id, Restaurant restaurant, List<IFormFile> images)
        {
            restaurant.res_id = id;

            var result = await _restaurantOwnerService.UpdateRestaurantAsync(restaurant, images);

            if (result != null)
            {
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.UPDATE_SUCCESS;
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Không thể cập nhật nhà hàng này vì có đơn đặt bàn đang diễn ra.";
            }

            return RedirectToAction(ControllerConstants.PROFILE_RESTAURANT, ControllerConstants.RESTAURANT_OWNER, new { id });
        }

        /// <summary>
        /// Creates a new restaurant owner.
        /// </summary>
        /// <param name="Name">The name of the restaurant owner.</param>
        /// <returns>Redirects to the restaurant owner's profile after creation.</returns>
        /// <author>Thangtm</author>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(string Name)
        {
            var cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (string.IsNullOrEmpty(Name))
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.RESTAURANT_NAME_REQUIRED;
                return RedirectToAction(ControllerConstants.PROFILE, ControllerConstants.RESTAURANT_OWNER);
            }
            if (!cusId.HasValue)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CUSTOMER_INFO_NOT_FOUND;
                return RedirectToAction(ControllerConstants.PROFILE, ControllerConstants.RESTAURANT_OWNER);
            }

            var restaurantOwner = new RestaurantOwner
            {
                cus_id = cusId.Value,
                res_owner_name = Name,
                res_owner_created_date = DateTime.Now,
                res_owner_follower_count = 0,
                res_owner_is_use = true
            };
            var response = await _apiService.PostAsync<ResOwnerResponse, dynamic>($"{ApiEndpoints.RESTAURANT_OWNER}", restaurantOwner);
            HttpContext.Session.SetInt32(SessionConstants.RESTAURANT_OWNER_ID, response.resOwner_id);
            return Ok(new { success = true, resOwner_id = response.resOwner_id });
        }

        [HttpGet("AccessRestaurant")]
        public IActionResult AccessRestaurant(int res_id)
        {
            HttpContext.Session.SetInt32(SessionConstants.RESTAURANT_ID, res_id);
            return RedirectToAction("ProfileRestaurant");
        }

        /// <summary>
        /// Displays the profile of a specific restaurant.
        /// </summary>
        /// <param name="res_id">The restaurant ID.</param>
        /// <returns>The restaurant profile view.</returns>
        /// <author>Thangtm</author>
        [HttpGet("ProfileRestaurant")]
        public async Task<IActionResult> ProfileRestaurant()
        {
            var res_id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_ID);
            if (!res_id.HasValue)
                return new RedirectToActionResult("Login", "Auth", null);

            var restaurant = await _restaurantService.GetRestaurantByID(res_id.Value);
            ViewBag.Restaurant = restaurant;
            return View();
        }

        [HttpGet("SetResOwner")]
        public IActionResult SetResOwner(int resOwner_id)
        {
            HttpContext.Session.SetInt32(SessionConstants.RESTAURANT_OWNER_ID, resOwner_id);

            return RedirectToAction("ListRestaurant");
        }

        [HttpGet("ListRestaurant")]
        public async Task<IActionResult> ListRestaurant()
        {
            var resOwner_id = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            var restaurants = await _restaurantService.GetALLRestaurantByResOwnerAsync(resOwner_id.Value);
            var owner = await _restaurantOwnerService.GetByIdAsync(resOwner_id.Value);

            var viewModel = new CustomRestaurantViewModel
            {
                Restaurants = restaurants ?? new List<Restaurant>(),
                RestaurantOwner = owner ?? new RestaurantOwner()
            };
            HttpContext.Session.SetString(SessionConstants.RESTAURANT_OWNER_NAME, owner.res_owner_name);
            return View(viewModel);
        }

        [HttpGet("CreateRestaurant")]
        public async Task<IActionResult> CreateRestaurant()
        {
            return View("CreateRestaurant", new Restaurant());
        }

        [HttpPost("CreateRestaurant")]
        public async Task<IActionResult> CreateRestaurant(Restaurant restaurant, List<IFormFile> images)
        {
            var resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            if (resOwnerId == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Không tìm thấy thông tin chủ nhà hàng.";
                return RedirectToAction("ListRestaurant");
            }
            restaurant.cate_id = 1;
            restaurant.res_owner_id = resOwnerId.Value;

            var result = await _restaurantOwnerService.CreateRestaurantAsync(restaurant, images);

            if (result != null)
            {
                TempData[KeyConstants.SUCCESS_MESSAGE] = "Tạo nhà hàng thành công!";
                return RedirectToAction("ListRestaurant", new { resOwner_id = resOwnerId });
            }

            TempData[KeyConstants.ERROR_MESSAGE] = "Tạo nhà hàng thất bại!";
            return View("CreateRestaurant", restaurant);
        }


        // [HttpGet("Chart")]
        // public async Task<IActionResult> ChartRestaurant()
        // {
        //     return View();
        // }

        [HttpGet("Verifications")]
        public async Task<IActionResult> Verifications()
        {
            var resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            // Lấy danh sách nhà hàng của chủ sở hữu
            var restaurants = await _restaurantService.GetALLRestaurantByResOwnerAsync(resOwnerId.Value);
            // Lấy tất cả verification của các nhà hàng này
            var verifications = new List<Verification>();
            foreach (var res in restaurants)
            {
                var list = await _restaurantOwnerService.GetVerificationsByRestaurantIdAsync(res.res_id);
                if (list != null) verifications.AddRange(list);
            }
            foreach (var verification in verifications)
            {
                verification.restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{verification.res_id}");

            }
            ViewBag.Restaurants = restaurants;
            return View(verifications);
        }

        [HttpGet("Verifications/Register")]
        public async Task<IActionResult> RegisterVerification()
        {
            var resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            if (resOwnerId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin chủ nhà hàng.";
                return RedirectToAction("Verifications");
            }
            var restaurants = await _restaurantService.GetALLRestaurantByResOwnerAsync(resOwnerId.Value);
            ViewBag.Restaurants = restaurants;
            return View(new Verification());
        }

        [HttpPost("Verifications/Register")]
        public async Task<IActionResult> RegisterVerification(Verification model, IFormFile file)
        {
            model.ver_date_submitted = DateTime.Now;
            model.ver_status = 0; // Chờ duyệt
            model.ver_is_deleted = false;
            await _restaurantOwnerService.RegisterVerificationAsync(model, file);
            TempData["SuccessMessage"] = "Đăng ký giấy phép thành công!";
            return RedirectToAction("Verifications");
        }

        [HttpGet("restaurantowner/messages")]
        public IActionResult ManageMessages()
        {
            return View();
        }

        [HttpGet("Chart")]
        public async Task<IActionResult> ChartRestaurant()
        {
            int resownerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID) ?? 0;
            var dashboardData = await _apiService.GetAsync<DashboardResownerViewModel>($"{ApiEndpoints.DASHBOARD_RESTAURANT_OWNER}/{resownerId}");
            return View(dashboardData);
        }

        [HttpGet("restaurantowner/ActiveAds")]
        // Trang 1: Quảng cáo đang active
        public async Task<IActionResult> ActiveAds()
        {
            int? resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            if (!resOwnerId.HasValue) return RedirectToAction("Login", "Auth");

            var ads = await _adService.GetAdsByStatusAsync(true);
            var myAds = ads.Where(a => a.res_owner_id == resOwnerId.Value).ToList();
            return View(myAds);
        }
        [HttpGet("restaurantowner/AvailableSlots")]
        // Trang 2: Slot đang có thể thuê
        public async Task<IActionResult> AvailableSlots()
        {
            var slots = await _adService.GetAllSlotsAsync();
            var available = slots.Where(s => s.slot_is_active && !s.occupied).ToList();
            return View(available);
        }

        [HttpGet("restaurantowner/InactiveAds")]
        // Trang 3: Quảng cáo đã hết hạn (inactive)
        public async Task<IActionResult> InactiveAds()
        {
            int? resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            if (!resOwnerId.HasValue) return RedirectToAction("Login", "Auth");

            await _adService.DeactivateExpiredAsync();

            var ads = await _adService.GetAdsByStatusAsync(false);
            var myAds = ads.Where(a => a.res_owner_id == resOwnerId.Value).ToList();
            return View(myAds);
        }

        [HttpGet("RegisterAd/{slotId}")]
        public async Task<IActionResult> RegisterAd(int slotId)
        {
            var resOwnerId = HttpContext.Session.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            if (!resOwnerId.HasValue) return RedirectToAction("Login", "Auth");

            var slots = await _adService.GetAllSlotsAsync();
            var target = slots.FirstOrDefault(s => s.slot_id == slotId);
            if (target == null) return NotFound();

            ViewBag.SlotPrice = target.slot_price;

            return View(new AdRegistrationRequestDto
            {
                slot_id = slotId,
                res_owner_id = resOwnerId.Value,
                start_date = DateTime.Now
            });
        }

        [HttpPost("RegisterAd")]
        public async Task<IActionResult> RegisterAdSubmit(AdRegistrationRequestDto dto, IFormFile imageFile, int duration)
        {
            if (imageFile == null)
            {
                TempData["ErrorMessage"] = "Bạn cần upload ảnh quảng cáo!";
                return RedirectToAction("RegisterAd", new { slotId = dto.slot_id });
            }

            var fileName = await _imageHelper.UploadImageWithThumbnailAsync(imageFile, "ads", 500);
            dto.ad_image_url = fileName;
            dto.end_date = dto.start_date.AddDays(duration);

            // 🔐 Token duy nhất cho phiên thanh toán này
            var token = Guid.NewGuid().ToString("N");

            // 💾 Lưu cả DTO và Token riêng trong Session
            HttpContext.Session.SetString("PendingAd", JsonConvert.SerializeObject(dto));
            HttpContext.Session.SetString("PendingAdToken", token);

            return RedirectToAction("PaymentAd");
        }

        [HttpPost("DetectExpiredAds")]
        public async Task<IActionResult> DetectExpiredAds()
        {
            try
            {
                var message = await _adService.DeactivateExpiredAsync();
                return Json(new { success = true, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpGet("PaymentAd")]
        public async Task<IActionResult> PaymentAd()
        {
            var raw = HttpContext.Session.GetString("PendingAd");
            var token = HttpContext.Session.GetString("PendingAdToken");

            if (string.IsNullOrEmpty(raw) || string.IsNullOrEmpty(token))
                return RedirectToAction("ActiveAds");

            var dto = JsonConvert.DeserializeObject<AdRegistrationRequestDto>(raw);

            var slots = await _adService.GetAllSlotsAsync();
            var slot = slots.FirstOrDefault(s => s.slot_id == dto.slot_id);
            if (slot == null) return RedirectToAction("ActiveAds");

            var duration = (dto.end_date - dto.start_date).Days;
            ViewBag.TotalPrice = slot.slot_price * duration;
            ViewBag.Token = token;

            return View("PaymentAd", dto);
        }

        [HttpGet("CheckPaidAd")]
        public async Task<IActionResult> CheckPaidAd()
        {
            var raw = HttpContext.Session.GetString("PendingAd");
            var token = HttpContext.Session.GetString("PendingAdToken");

            if (string.IsNullOrEmpty(raw) || string.IsNullOrEmpty(token))
                return Ok(new { success = false, message = "❌ Không có quảng cáo pending." });

            var dto = JsonConvert.DeserializeObject<AdRegistrationRequestDto>(raw);

            var slots = await _adService.GetAllSlotsAsync();
            var slot = slots.FirstOrDefault(s => s.slot_id == dto.slot_id);
            if (slot == null)
                return Ok(new { success = false, message = "Không tìm thấy slot." });

            var duration = (dto.end_date - dto.start_date).Days;
            var totalPrice = slot.slot_price * duration;

            var jsonDoc = await _apiService.GetAsync<JsonDocument>($"{ApiEndpoints.PAYMENT}/GetTransactions");
            var obj = JObject.Parse(jsonDoc.RootElement.GetRawText());
            var records = obj["data"]?["records"] as JArray;

            if (records == null)
                return Ok(new { success = false, message = "Không có giao dịch nào." });

            foreach (var tx in records)
            {
                var desc = (string?)tx["description"] ?? "";
                var amount = (decimal?)tx["amount"] ?? 0;

                if (desc.ToLower().Contains($"adtoken {token}".ToLower()) && amount == totalPrice)
                {
                    await _adService.RegisterAdAsync(dto);
                    HttpContext.Session.Remove("PendingAd");
                    HttpContext.Session.Remove("PendingAdToken");
                    return Ok(new { success = true, message = "✅ Thanh toán thành công, quảng cáo đã được kích hoạt." });
                }
            }

            return Ok(new { success = false, message = "⌛ Chưa tìm thấy giao dịch phù hợp." });
        }

        [HttpPut("CancelIfExpiredAd")]
        public IActionResult CancelIfExpiredAd()
        {
            HttpContext.Session.Remove("PendingAd");
            HttpContext.Session.Remove("PendingAdToken");
            return Ok(new { success = true, message = "🗑 Quảng cáo pending đã bị hủy do hết hạn thanh toán." });
        }

    }
}