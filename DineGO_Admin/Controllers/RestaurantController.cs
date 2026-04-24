using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Models.AdminModel;
using Core.Common;

namespace DineGO_Admin.Controllers
{

    public class RestaurantController : Controller
    {
        private readonly RestaurantService _restaurantService;
        private readonly CategoryService _categoryService;
        private readonly RestaurantOwnerService _restaurantOwnerService;
        private readonly MenuService _menuService;
        private readonly FoodService _foodService;

        public RestaurantController(RestaurantService restaurantService, CategoryService categoryService, RestaurantOwnerService restaurantOwnerService, MenuService menuService, FoodService foodService)
        {
            _restaurantService = restaurantService;
            _categoryService = categoryService;
            _restaurantOwnerService = restaurantOwnerService;
            _menuService = menuService;
            _foodService = foodService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 6)
        {
            var restaurants = await _restaurantService.GetAllRestaurantsForAdminAsync();
            var pagedList = PaginatedList<Restaurant>.Create(restaurants, pageIndex, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetail(int id)
        {
            var menus = await _menuService.GetMenusByRestaurantAsync(id);
            foreach (var menu in menus)
            {
                menu.foods = await _foodService.GetFoodsByMenuIdAsync(menu.menu_id);
            }
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new RestaurantViewDetailModel
            {
                Menus = menus,
                Restaurant = restaurant
            };
            
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> AddRestaurant()
        {
            var categories = await _categoryService.GetAllAsync();
            var restaurantOwners = await _restaurantOwnerService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.RestaurantOwners = restaurantOwners;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRestaurant(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                var response = await _restaurantService.AddRestaurantAsync(restaurant);
                if (response != null)
                {
                    TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.RESTAURANT_CREATE_SUCCESS;
                }
                else
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.RESTAURANT_CREATE_FAILED;
                }
                return RedirectToAction("Index");
            }
            TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATA_INVALID;
            var categories = await _categoryService.GetAllAsync();
            var restaurantOwners = await _restaurantOwnerService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.RestaurantOwners = restaurantOwners;
            return View(restaurant);
        }
        public async Task<IActionResult> UpdateRestaurant(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.RESTAURANT_WITH_ID_NOT_FOUND;
                return RedirectToAction("Index");
            }
            var categories = await _categoryService.GetAllAsync();
            var restaurantOwners = await _restaurantOwnerService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.RestaurantOwners = restaurantOwners;
            return View(restaurant);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRestaurant(Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                var updatedRestaurant = await _restaurantService.UpdateRestaurantAsync(restaurant);
                if (updatedRestaurant)
                {
                    TempData[KeyConstants.SUCCESS_MESSAGE] = "Cập nhật nhà hàng thành công.";
                }
                else
                {
                    TempData[KeyConstants.ERROR_MESSAGE] = "Cập nhật nhà hàng thất bại.";
                }
                return RedirectToAction("Index");
            }
            TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATA_INVALID;
            var categories = await _categoryService.GetAllAsync();
            var restaurantOwners = await _restaurantOwnerService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.RestaurantOwners = restaurantOwners;
            return View(restaurant);
        }

        [HttpPost]
        public async Task<IActionResult> BlockRestaurant(int id)
        {
            var result = await _restaurantService.BlockRestaurantAsync(id);
            if (result)
            {
                TempData[KeyConstants.SUCCESS_MESSAGE] = "Restaurant blocked successfully.";
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Failed to block restaurant.";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ActiveRestaurant(int id)
        {
            var result = await _restaurantService.ActiveRestaurantAsync(id);
            if (result)
            {
                TempData[KeyConstants.SUCCESS_MESSAGE] = "Restaurant activated successfully.";
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Failed to activate restaurant.";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var result = await _restaurantService.DeleteRestaurantAsync(id); // Xóa mềm: set res_is_use = false
            if (result)
            {
                TempData[KeyConstants.SUCCESS_MESSAGE] = "Xóa nhà hàng thành công.";
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Xóa nhà hàng thất bại.";
            }
            return RedirectToAction("Index");
        }
    }
}