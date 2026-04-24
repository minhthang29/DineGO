using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Newtonsoft.Json;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles menu management operations for restaurant owners in the client application.
    /// </summary>
    /// <author>KhoiNV</author>
    [Route("[controller]")]
    public class MenuController : BaseController
    {
        private readonly MenuService _menuService;
        private readonly FoodService _foodService;
        private readonly ImageHelper _imageHelper;

        public MenuController(MenuService menuService,FoodService foodService, ImageHelper imageHelper)
        {
            _menuService = menuService;
            _foodService = foodService;
            _imageHelper = imageHelper;
        }

        /// <summary>
        /// Displays the list of menus belonging to a specific restaurant.
        /// </summary>
        /// <param name="res_id">Restaurant ID</param>
        /// <returns>View with list of menus</returns>
       [HttpGet("ListMenu/{res_id}")]
public async Task<IActionResult> ListMenu(int res_id)
{
    var menus = await _menuService.GetMenusByRestaurantAsync(res_id);
    var restaurant = await _restaurantService.GetRestaurantByID(res_id);

    var vm = new MenuWithRestaurantViewModel
    {
        Menus = menus,
        Restaurant = restaurant,
        MenuFoodCounts = new Dictionary<int, int>()
    };

    foreach (var menu in menus)
    {
        var foods = await _foodService.GetFoodsByMenuIdAsync(menu.menu_id);
        vm.MenuFoodCounts[menu.menu_id] = foods.Count;
    }

    return View(vm);
}

        /// <summary>
        /// Renders the create menu form for a given restaurant.
        /// </summary>
        /// <param name="res_id">Restaurant ID</param>
        [HttpGet("Create/{res_id}")]
        public async Task<IActionResult> Create(int res_id)
        {
            var restaurant = await _restaurantService.GetRestaurantByID(res_id);
            var vm = new MenuWithRestaurantViewModel
            {
                Restaurant = restaurant,
                Menu = new Menu { res_id = res_id }
            };
            return View(vm);
        }
        /// <summary>
        /// Handles POST request to create a new menu. Supports image upload.
        /// </summary>
        /// <param name="vm">ViewModel containing menu data</param>
        /// <param name="imageFile">Uploaded menu image</param>
        [HttpPost("Create/{res_id}")]
        public async Task<IActionResult> Create(MenuWithRestaurantViewModel vm, List<IFormFile> imageFiles)
        {
            var imageNames = new List<string>();

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = await _imageHelper.UploadImageWithThumbnailAsync(file, "menus", thumbWidth: 300);
                        imageNames.Add(fileName);
                    }
                }
                vm.Menu.menu_image = JsonConvert.SerializeObject(imageNames); // lưu chuỗi JSON
            }

            await _menuService.CreateMenuAsync(vm.Menu);
            return RedirectToAction("ListMenu", new { res_id = vm.Menu.res_id });
        }


        /// <summary>
        /// Displays the form to edit a specific menu.
        /// </summary>
        /// <param name="id">Menu ID</param>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null) return NotFound();

            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            var vm = new MenuWithRestaurantViewModel
            {
                Menu = menu,
                Restaurant = restaurant
            };

            return View(vm);
        }

        /// <summary>
        /// Handles POST request to update an existing menu.
        /// </summary>
        /// <param name="vm">ViewModel containing updated menu data</param>
        /// <param name="imageFile">Optional image file to replace existing image</param>
        [HttpPost("Edit/{id}")]
public async Task<IActionResult> Edit(MenuWithRestaurantViewModel vm, List<IFormFile> imageFiles)
{
    if (vm.Menu == null)
    {
        ModelState.AddModelError("", "Menu is null");
        return View(vm);
    }

    var menuInDb = await _menuService.GetMenuByIdAsync(vm.Menu.menu_id);
    var imageNames = new List<string>();

    if (imageFiles != null && imageFiles.Count > 0)
    {
        foreach (var file in imageFiles)
        {
            if (file.Length > 0)
            {
                var fileName = await _imageHelper.UploadImageWithThumbnailAsync(file, "menus", 300);
                imageNames.Add(fileName);
            }
        }
        vm.Menu.menu_image = JsonConvert.SerializeObject(imageNames);
    }
    else
    {
        // ✅ Giữ nguyên ảnh cũ nếu không upload mới
        vm.Menu.menu_image = menuInDb.menu_image;
    }

    await _menuService.UpdateMenuAsync(vm.Menu);
    return RedirectToAction("ListMenu", new { res_id = vm.Menu.res_id });
}


        /// <summary>
        /// Displays a confirmation view before deleting a menu.
        /// </summary>
        /// <param name="id">Menu ID</param>
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null) return NotFound();

            var restaurant = await _restaurantService.GetRestaurantByID(menu.res_id);
            var vm = new MenuWithRestaurantViewModel
            {
                Menu = menu,
                Restaurant = restaurant
            };

            return View(vm);
        }
        /// <summary>
        /// Executes the deletion (soft delete) of the menu.
        /// </summary>
        /// <param name="id">Menu ID</param>
        /// <param name="res_id">Restaurant ID for redirection</param>
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id, int res_id)
        {
            var menu = await _menuService.GetMenuByIdAsync(id);
            if (menu == null) return NotFound();

            await _menuService.DeleteMenuAsync(menu);

            return RedirectToAction("ListMenu", new { res_id });
        }


    }
}