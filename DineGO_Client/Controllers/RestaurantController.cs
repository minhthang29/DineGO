using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models.Client.Custom;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles actions related to restaurants, such as listing, viewing details, and searching.
    /// </summary>
    ///  <author>Thangtm;Khoinv</author>
    public class RestaurantController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApiService _apiService;
        private readonly RestaurantService _restaurantService;
        private readonly MenuService _menuService;
        private readonly FoodService _foodService;
        private readonly RatingService _ratingService;
        private readonly ChatService _chatService;
        private readonly RestaurantOwnerService _restaurantOwnerService;

        public RestaurantController(
            ILogger<AuthController> logger,
            ApiService apiService,
            RestaurantService restaurantService,
            MenuService menuService,
            FoodService foodService,
            RatingService ratingService,
            ChatService chatService,
            RestaurantOwnerService restaurantOwnerService)
        {
            _logger = logger;
            _apiService = apiService;
            _restaurantService = restaurantService;
            _menuService = menuService;
            _foodService = foodService;
            _ratingService = ratingService;
            _chatService = chatService;
            _restaurantOwnerService = restaurantOwnerService;
        }

        /// <summary>
        /// Displays a list of all restaurants.
        /// </summary>
        /// <returns>Returns the view with a list of restaurants.</returns>
        /// <author>Thangtm;Khoinv</author>
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<Restaurant>>(ApiEndpoints.RESTAURANT);
            var restaurants_is_use = response.Where(r => r.res_is_use).ToList();
            return View(restaurants_is_use);
        }

        /// <summary>
        /// Displays the details of a specific restaurant.
        /// </summary>
        /// <param name="id">The restaurant ID.</param>
        /// <returns>Returns the view with the restaurant details.</returns>
        /// <author>Thangtm</author>
        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByID(id);
            var menus = await _menuService.GetMenusByRestaurantAsync(id);
            var ratings = await _ratingService.GetRatingsByRestaurantIdAsync(id);
            var resOwnerId = await _chatService.GetResOwnerIdByResIdAsync(id);
            var resOwner = await _restaurantOwnerService.GetByIdAsync((int)resOwnerId);
            ViewBag.ResOwnerName = resOwner?.res_owner_name ?? "Ẩn danh";

            var customerId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);

            bool hasCompletedOrder = false;
            List<Customer>? allCustomers = new List<Customer>();

            if (customerId.HasValue)
            {
                hasCompletedOrder = await _ratingService.HasCompletedOrderAsync(customerId.Value, id);
                allCustomers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            }


            var viewModel = new RestaurantDetailViewModel
            {
                Restaurant = restaurant,
                MenusWithFoods = new List<MenuWithFoodsViewModel>(),
                Ratings = ratings,
                Customers = allCustomers,
                CurrentCustomerId = customerId,
                HasCompletedOrder = hasCompletedOrder
            };

            foreach (var menu in menus)
            {
                var foods = await _foodService.GetFoodsByMenuIdAsync(menu.menu_id);
                viewModel.MenusWithFoods.Add(new MenuWithFoodsViewModel
                {
                    Menu = menu,
                    Foods = foods
                });
            }

            return View(viewModel);
        }

        /// <summary>
        /// Creates or updates a rating for a restaurant.
        /// </summary>
        /// <param name="model">Rating view model</param>
        /// <returns>Status with rating ID</returns>
        /// <author>KhoiNV</author>
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateRating(RestaurantRatingViewModel model)
        {
            try
            {
                var result = await _ratingService.AddOrUpdateRatingAsync(model);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.RATING_SUCCESS;
                return RedirectToAction("Details", new { id = model.res_id });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Details", new { id = model.res_id });
            }
        }



        /// <summary>
        /// Searches for restaurants by name and/or address.
        /// </summary>
        /// <param name="name">The name of the restaurant.</param>
        /// <param name="address">The address of the restaurant.</param>
        /// <returns>Returns the view with the search results.</returns>
        /// <author>Khoinv</author>
        public async Task<IActionResult> Search(string name, string address)
        {
            string searchUrl = string.Format(ApiEndpoints.RESTAURANT_SEARCH, name ?? "", address ?? "");
            var restaurants = await _apiService.GetAsync<List<Restaurant>>(searchUrl);

            // Check the data returned from the API
            if (restaurants == null || restaurants.Count == 0)
            {
                return View(new List<Restaurant>()); // Returns an empty list if there are no results
            }

            return View(restaurants); // Returns a view that displays search results.
        }

        /// <summary>
        /// Searches for restaurants by name and/or address using ajax to update realtime
        /// </summary>
        /// <param name="name">The name of the restaurant.</param>
        /// <param name="address">The address of the restaurant.</param>
        /// <returns>Returns the view with the search results.</returns>
        /// <author>Thangtm</author>
        public async Task<IActionResult> SearchByLocation(string name, string address)
        {
            string searchUrl = string.Format(ApiEndpoints.RESTAURANT_SEARCH, name ?? "", address ?? "");
            var restaurants = await _apiService.GetAsync<List<Restaurant>>(searchUrl);

            return PartialView("_RestaurantListPartial", restaurants);
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int res_id)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            try
            {
                var success = await _restaurantService.FollowRestaurantAsync(cus_id.Value, res_id);
                if (success)
                    return Json(new { success = true, message = "Đã theo dõi nhà hàng." });
                else
                    return Json(new { success = false, message = "Bạn đã theo dõi nhà hàng này." });
            }
            catch
            {
                return Json(new { success = false, message = "Lỗi khi theo dõi nhà hàng." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int res_id)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return Json(new { success = false, message = "Bạn chưa đăng nhập." });

            try
            {
                var success = await _restaurantService.UnfollowRestaurantAsync(cus_id.Value, res_id);
                if (success)
                    return Json(new { success = true, message = "Đã bỏ theo dõi nhà hàng." });
                else
                    return Json(new { success = false, message = "Bạn chưa theo dõi nhà hàng này." });
            }
            catch
            {
                return Json(new { success = false, message = "Lỗi khi bỏ theo dõi." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckFollow(int res_id)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return Json(new { isFollowing = false });

            try
            {
                var isFollowing = await _restaurantService.IsFollowingRestaurantAsync(cus_id.Value, res_id);
                return Json(new { isFollowing });
            }
            catch
            {
                return Json(new { isFollowing = false });
            }
        }
    }
}