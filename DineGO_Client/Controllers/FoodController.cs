using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using Core.Helper;
namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Controller that handles actions related to the food.
    /// </summary>
    /// <author>KhoiNV</author>
    public class FoodController : Controller
    {
        private readonly ILogger<FoodController> _logger;
        private readonly FoodService _foodService;
        private readonly CartService _cartService;
        private readonly AIService _aIService;
        private readonly MenuService _menuService;

        private readonly ImageHelper _imageHelper;
        private readonly CategoryService _categoryService;
        private readonly RestaurantService _restaurantService;

        public FoodController(ILogger<FoodController> logger, FoodService foodService, CartService cartService, MenuService menuService, ImageHelper imageHelper, AIService aIService, CategoryService categoryService, RestaurantService restaurantService)
        {
            _logger = logger;
            _foodService = foodService;
            _cartService = cartService;
            _menuService = menuService;
            _aIService = aIService;
            _imageHelper = imageHelper;
            _categoryService = categoryService;
            _restaurantService = restaurantService;
        }
        public async Task<IActionResult> Index()
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            double? lat = null, lng = null;

            if (double.TryParse(HttpContext.Session.GetString("USER_LAT"), out var savedLat))
                lat = savedLat;
            if (double.TryParse(HttpContext.Session.GetString("USER_LNG"), out var savedLng))
                lng = savedLng;

            var groupedFoods = await _foodService.SearchFoodsAsync(null, null, null, null, null, lat, lng);
            List<CartItemViewModel> cart = new List<CartItemViewModel>();
            if (cusId != null)
                cart = await _cartService.GetCartItems(cusId.Value);

            var markers = await _foodService.GetRestaurantMarkersAsync();
            ViewBag.RestaurantMarkers = markers;
            ViewBag.Cart = cart;

            return View(groupedFoods);
        }


        /// <summary>
        /// Search food by name, restaurant name , min and max price.
        /// </summary>
        /// <return> list of food</return>
        /// <author>KhoiNV</author>
        [HttpGet]
        public async Task<IActionResult> Search(string? keyword, string? restaurantName, decimal? minPrice, decimal? maxPrice, string? userAddress, double? userLat, double? userLng)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (userLat.HasValue && userLng.HasValue)
            {
                HttpContext.Session.SetString("USER_LAT", userLat.Value.ToString());
                HttpContext.Session.SetString("USER_LNG", userLng.Value.ToString());
            }

            // 🔁 Tạo task cho tìm kiếm món ăn
            var searchTask = _foodService.SearchFoodsAsync(keyword, restaurantName, minPrice, maxPrice, userAddress);

            // 🔁 Nếu có cusId, tạo task ghi nhận ưu tiên (chạy ngầm, không blocking)
            Task? priorityTask = null;
            if (cusId.HasValue && !string.IsNullOrWhiteSpace(keyword))
            {
                priorityTask = Task.Run(async () =>
                {
                    try
                    {
                        await _aIService.UpdatePriorityAsync(cusId.Value, keyword);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Không thể cập nhật priority: " + ex.Message);
                    }
                });
            }

            // ⏳ Đợi task tìm kiếm chính
            var foods = await searchTask;
            var markers = await _foodService.GetRestaurantMarkersAsync();
            ViewBag.RestaurantMarkers = markers;
            foods = await _foodService.SearchFoodsAsync(keyword, restaurantName, minPrice, maxPrice, userAddress, userLat, userLng);
            return View("Index", foods);
        }

        /// <summary>
        /// Details of food and other foods of same restaurant
        /// </summary>
        /// <return> details of food and other foods</return>
        /// <author>KhoiNV</author>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null) return NotFound();

            var menu = await _menuService.GetMenuByIdAsync(food.menu_id);
            if (menu == null) return NotFound();

            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            if (restaurant == null) return NotFound();
            var relatedFoods = await _foodService.GetFoodsByMenuIdAsync(food.menu_id);
            ViewBag.RelatedFoods = relatedFoods.Where(f => f.food_id != id).ToList();
            ViewBag.RestaurantName = restaurant.res_name;

            // ✅ Ghi nhận click nếu có cusId và tag
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId.HasValue && !string.IsNullOrWhiteSpace(food.food_tag))
            {
                var tags = food.food_tag
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(t => t.Trim().ToLowerInvariant())
                    .Distinct();

                // 🔁 Tạo danh sách task ghi click
                var clickTasks = tags.Select(tag => Task.Run(async () =>
                {
                    try
                    {
                        await _aIService.AddClickToTagAsync(cusId.Value, tag);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Không thể ghi nhận click tag: " + tag + ". Lỗi: " + ex.Message);
                    }
                })).ToList();

                // 🔁 Không cần await: để chạy song song như Search
                _ = Task.WhenAll(clickTasks);
            }

            return View(food);
        }
        /// <summary>
        /// Manage food follow by menu id
        /// </summary>
        /// <return> list of foods in menu</return>
        /// <author>KhoiNV</author>
        [HttpGet]
        public async Task<IActionResult> ManageByMenu(int menuId)
        {
            var foods = await _foodService.GetFoodsByMenuIdAsync(menuId);
            var menu = await _menuService.GetMenuByIdAsync(menuId);
            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            ViewBag.Restaurant = restaurant;
            var vm = new FoodWithMenuViewModel
            {
                Foods = foods,
                Menu = menu,
                Restaurant = restaurant
            };

            return View("ManageByMenu", vm);
        }

        /// <summary>
        /// Renders the create food form for a given menu.
        /// </summary>
        /// <param name="menuId">Menu ID</param>
        /// <returns>Form view for creating a new food item.</returns>
        /// <author>KhoiNV</author>
        [HttpGet("Create/{menuId}")]
        public async Task<IActionResult> Create(int menuId)
        {
            var menu = await _menuService.GetMenuByIdAsync(menuId);
            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            if (menu == null || restaurant == null)
                return NotFound();
            ViewBag.Restaurant = restaurant;
            var categories = await _categoryService.GetAllAsync();
            ViewBag.TagCategories = categories
            .Where(c => !string.IsNullOrEmpty(c.cate_description) && c.cate_description.StartsWith("AI"))
            .Select(c => c.cate_type)
            .ToList();

            var vm = new FoodWithMenuViewModel
            {
                Menu = menu,
                Foods = new List<Food> { new Food { menu_id = menuId } },
                Restaurant = restaurant
            };

            return View(vm);
        }

        // 🔥 Endpoint nội bộ gọi AIService thay vì gọi trực tiếp API từ JS
        [HttpGet("/Food/SuggestTagsFromDescription")] // ← thêm route tuyệt đối
        public async Task<IActionResult> SuggestTagsFromDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return BadRequest("Description is required");

            try
            {
                var tags = await _aIService.SuggestTagsFromTextAsync(description);
                return Json(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi gọi AI: {ex.Message}");
            }
        }
        /// <summary>
        /// Handles POST request to create a new food item. Supports image upload.
        /// </summary>
        /// <param name="vm">ViewModel containing food and menu data</param>
        /// <param name="imageFile">Uploaded food image</param>
        /// <returns>Redirect to menu management page</returns>
        /// <author>KhoiNV</author>
        [HttpPost("Create/{menuId}")]
        public async Task<IActionResult> Create(FoodWithMenuViewModel vm, List<IFormFile> imageFiles)
        {
            var food = vm.Foods?.FirstOrDefault();
            if (food == null) return View(vm);

            var imageNames = new List<string>();
            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var image in imageFiles)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(image, "foods", 300);
                    imageNames.Add(fileName);
                }

                food.food_image = JsonSerializer.Serialize(imageNames);
            }

            food.menu_id = vm.Menu.menu_id;
            await _foodService.CreateFoodAsync(food);
            return RedirectToAction("ManageByMenu", new { menuId = food.menu_id });
        }


        /// <summary>
        /// Displays the form to edit a specific food item.
        /// </summary>
        /// <param name="id">Food ID</param>
        /// <author>KhoiNV</author>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null)
                return NotFound();

            var menu = await _menuService.GetMenuByIdAsync(food.menu_id);
            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            if (menu == null || restaurant == null)
                return NotFound();
            ViewBag.Restaurant = restaurant;
            // Lấy danh sách tag phổ biến (cate_type bắt đầu bằng "AI")
            var categories = await _categoryService.GetAllAsync();
            ViewBag.TagCategories = categories
                .Where(c => c.cate_description != null && c.cate_description.StartsWith("AI"))
                .Select(c => c.cate_type)
                .Distinct()
                .ToList();

            var vm = new FoodWithMenuViewModel
            {
                Menu = menu,
                Restaurant = restaurant,
                Foods = new List<Food> { food }
            };

            return View(vm);
        }


        /// <summary>
        /// Handles POST request to update a food item. Supports multiple image replacement.
        /// </summary>
        /// <param name="vm">ViewModel with updated food</param>
        /// <param name="imageFiles">New images if provided</param>
        /// <author>KhoiNV</author>
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(FoodWithMenuViewModel vm, List<IFormFile> imageFiles)
        {
            var food = vm.Foods?.FirstOrDefault();
            if (food == null) return BadRequest();

            if (imageFiles != null && imageFiles.Any())
            {
                var imageNames = new List<string>();

                foreach (var image in imageFiles)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(image, "foods", 300);
                    imageNames.Add(fileName);
                }

                food.food_image = JsonSerializer.Serialize(imageNames);
            }
            else
            {

                var foodInDb = await _foodService.GetFoodByIdAsync(food.food_id);
                food.food_image = foodInDb.food_image;
            }

            await _foodService.UpdateFoodAsync(food);
            return RedirectToAction("ManageByMenu", new { menuId = vm.Menu.menu_id });
        }


        /// <summary>
        /// Displays a confirmation view before deleting a food item.
        /// </summary>
        /// <param name="id">Food ID</param>
        /// <author>KhoiNV</author>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null) return NotFound();

            var menu = await _menuService.GetMenuByIdAsync(food.menu_id);
            if (menu == null) return NotFound();

            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            if (restaurant == null) return NotFound();
            ViewBag.Restaurant = restaurant;
            var vm = new FoodWithMenuViewModel
            {
                Foods = new List<Food> { food },
                Menu = menu,
                Restaurant = restaurant
            };

            return View(vm);
        }
        /// <summary>
        /// Executes the soft deletion of a food item.
        /// </summary>
        /// <param name="id">Food ID</param>
        /// <param name="menuId">Menu ID for redirection</param>
        /// <author>KhoiNV</author>
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id, int menuId)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null) return NotFound();

            food.food_is_deleted = true; // ✅ Gán trực tiếp
            await _foodService.UpdateFoodAsync(food); // ✅ Gọi update

            return RedirectToAction("ManageByMenu", new { menuId });
        }

    }
}