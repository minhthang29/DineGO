using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Services;
using Core.Constant;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Core.Common;

namespace DineGO_Admin.Controllers

{

    public class RestaurantOwnerController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<RestaurantOwnerController> _logger;

        public RestaurantOwnerController(ApiService apiService, ILogger<RestaurantOwnerController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 6)
        {
            var response = await _apiService.GetAsync<List<RestaurantOwner>>(ApiEndpoints.RESTAURANT_OWNER);
            foreach (var r in response)
            {
                var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{r.cus_id}");
                r.customer = customer;
            }
            var pagedList = PaginatedList<RestaurantOwner>.Create(response, pageIndex, pageSize);
            return View(pagedList);
        }
        public async Task<IActionResult> Create()
        {
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RestaurantOwner owner)
        {
            if (ModelState.IsValid)
            {
                owner.res_owner_created_date = DateTime.Now;
                var response = await _apiService.PostAsync<RestaurantOwner, RestaurantOwner>(ApiEndpoints.RESTAURANT_OWNER, owner);
                if (response != null)
                {
                    TempData["SuccessMessage"] = "Thêm thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Thêm thất bại!";
            }
            return View(owner);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var owner = await _apiService.GetAsync<RestaurantOwner>($"{ApiEndpoints.RESTAURANT_OWNER}/id?Id={id}");
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            ViewBag.Customers = customers;
            if (owner == null)
                return NotFound();
            return View(owner);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RestaurantOwner owner)
        {
            if (ModelState.IsValid)
            {
                owner.res_owner_created_date = DateTime.Now;
                var response = await _apiService.PutAsync<RestaurantOwner, RestaurantOwner>($"{ApiEndpoints.RESTAURANT_OWNER}", owner);
                if (response != null)
                {
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Cập nhật thất bại!";
            }
            return View(owner);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            System.Console.WriteLine("Debug");
            var response = await _apiService.DeleteAsync<object>($"{ApiEndpoints.RESTAURANT_OWNER}?Id={id}");
            if (response != null)
            {
                TempData["SuccessMessage"] = "Xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa thất bại!";
            }
            return RedirectToAction("Index");
        }
    }
}
