using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Models.AdminModel.Custom;
using Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace DineGO_Admin.Controllers
{

    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ApiService _apiService;
        private readonly HashService _hashService;
        private readonly ImageHelper _imageHelper;
        private readonly S3BucketAWS _S3;

        public CustomerController(ILogger<CustomerController> logger, ApiService apiService, HashService hashService, ImageHelper imageHelper, S3BucketAWS S3)
        {
            _logger = logger;
            _apiService = apiService;
            _hashService = hashService;
            _imageHelper = imageHelper;
            _S3 = S3;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 6)
        {
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            var pagedList = PaginatedList<Customer>.Create(customers, pageIndex, pageSize);
            return View(pagedList);
        }
        [HttpGet]
        public IActionResult AddCustomer()
        {
            var customer = new Customer();
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer, IFormFile cus_image)
        {
            if (!ModelState.IsValid)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATA_INVALID;
                return View(customer);
            }
            customer.cus_is_use = true;
            customer.cus_password = _hashService.HashPassword(customer.cus_password);

            // Xử lý upload ảnh
            if (cus_image != null && cus_image.Length > 0)
            {
                var fileName = await _imageHelper.UploadImageWithThumbnailAsync(cus_image, "customers", thumbWidth: 600);
                customer.cus_image = fileName;
            }
            else
            {
                customer.cus_image = "default.jpeg";
            }

            try
            {
                var response = await _apiService.PostAsync<object, dynamic>(ApiEndpoints.CUSTOMER, customer);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CUSTOMER_CREATE_SUCCESS;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCustomer(int id)
        {
            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{id}");
            if (customer == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CUSTOMER_WITH_ID_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(CustomerUpdateProfileViewModel customer, IFormFile cus_image)
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    System.Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }
            if (!ModelState.IsValid)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.DATA_INVALID;
                return View(customer);
            }
            // Xử lý upload ảnh
            if (cus_image != null && cus_image.Length > 0)
            {
                var fileName = await _imageHelper.UploadImageWithThumbnailAsync(cus_image, "customers", thumbWidth: 600);
                customer.cus_image = fileName;
            }
            try
            {
                var response = await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.CUSTOMER}/{customer.cus_id}", customer);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CUSTOMER_UPDATE_SUCCESS;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{id}");
            if (customer == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CUSTOMER_WITH_ID_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            var response = await _apiService.DeleteAsync<dynamic>($"{ApiEndpoints.CUSTOMER}/{id}");
            TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.CUSTOMER_DELETE_SUCCESS;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> BlockCustomer(int id)
        {
            try
            {
                var response = await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.CUSTOMER}/block/{id}", null);
                TempData[KeyConstants.SUCCESS_MESSAGE] = "Khách hàng đã bị ngừng hoạt động.";
            }
            catch (HttpRequestException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ActivateCustomer(int id, string activate_reason, bool send_email, DateTime activate_date, string activated_by)
        {
            try
            {
                // Tạo object chứa thông tin kích hoạt (nếu API backend cần)
                var activateInfo = new
                {
                    activate_reason,
                    send_email,
                    activate_date,
                    activated_by
                };

                // Gọi API để kích hoạt tài khoản
                var response = await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.CUSTOMER}/activate/{id}", activateInfo);

                TempData[KeyConstants.SUCCESS_MESSAGE] = "Kích hoạt tài khoản thành công.";
            }
            catch (HttpRequestException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}