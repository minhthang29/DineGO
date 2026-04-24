using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Common;

namespace DineGO_Admin.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApiService _apiService;

        public NotificationController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _apiService.GetAsync<List<Notification>>("Notification");
            return View(notifications);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Notification notification)
        {
            if (!ModelState.IsValid) return View(notification);

            notification.noti_date = DateTime.Now;

            var createdNotification = await _apiService.PostAsync<Notification, Notification>("Notification", notification);

            if (createdNotification != null)
            {
                TempData["SuccessMessage"] = "Thêm thông báo thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo thông báo!";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var notification = await _apiService.GetAsync<Notification>($"Notification/{id}");
            if (notification == null) return NotFound();
            return View(notification);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Notification notification)
        {
            if (!ModelState.IsValid) return View(notification);

            var result = await _apiService.PutAsync<object, dynamic>($"Notification/{notification.noti_id}", notification);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Cập nhật thông báo thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông báo!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteAsync<object>($"Notification/{id}");
                TempData["SuccessMessage"] = "Xóa thông báo thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // 👇 Action để gửi notification cho customer cụ thể
        public IActionResult SendToCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendToCustomer(int notiId, int cusId)
        {
            try
            {
                var notification = await _apiService.GetAsync<Notification>($"Notification/{notiId}");
                if (notification == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông báo!";
                    return RedirectToAction("SendToCustomer");
                }

                var notificationCustomer = new NotificationCustomer
                {
                    noti_id = notiId,
                    cus_id = cusId,
                    noti_customer_is_read = false,
                    noti_customer_send_date = DateTime.Now,
                    order_id = null
                };

                var result = await _apiService.PostAsync<NotificationCustomer, NotificationCustomer>("NotificationCustomer", notificationCustomer);

                if (result != null)
                {
                    TempData["SuccessMessage"] = $"Đã gửi thông báo cho customer {cusId}!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi thông báo!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        
        public IActionResult ActionGuide()
        {
            return View();
        }
    }
}