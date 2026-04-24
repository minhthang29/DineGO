using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Client.Controllers
{
    [Route("[controller]")]
    public class NotificationController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ApiService apiService, ILogger<NotificationController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách thông báo theo customerId (dùng cho fetch từ JS)
        /// </summary>
        [HttpGet("GetLatest")]
        public async Task<IActionResult> GetLatest(int cusId)
        {
            try
            {
                var notifications = await _apiService.GetAsync<List<Notification>>($"Notification/by-customer/{cusId}");
                return Json(notifications ?? new List<Notification>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications for customer {CusId}", cusId);
                return Json(new List<Notification>());
            }
        }

        /// <summary>
        /// Đánh dấu một thông báo đã đọc
        /// </summary>
        [HttpPost("MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            try
            {
                if (request == null || request.NotiId <= 0 || request.CusId <= 0)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Thông tin không hợp lệ" 
                    });
                }

                // Gọi API Server qua ApiService
                var result = await _apiService.PostAsync<object, object>("Notification/mark-as-read", request);
                
                return Json(new 
                { 
                    success = true, 
                    message = "Đánh dấu đã đọc thành công",
                    notiId = request.NotiId,
                    cusId = request.CusId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read: NotiId={NotiId}, CusId={CusId}", 
                    request?.NotiId, request?.CusId);
                
                return Json(new 
                { 
                    success = false, 
                    message = "Lỗi khi đánh dấu thông báo đã đọc" 
                });
            }
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        [HttpPost("MarkAllAsRead/{cusId}")]
        public async Task<IActionResult> MarkAllAsRead(int cusId)
        {
            try
            {
                if (cusId <= 0)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Customer ID không hợp lệ" 
                    });
                }

                // Gọi API Server qua ApiService
                var result = await _apiService.PostAsync<MarkAllAsReadResponse, object>($"Notification/mark-all-as-read/{cusId}", null);
                
                return Json(new 
                { 
                    success = true, 
                    message = "Đánh dấu tất cả thông báo đã đọc thành công",
                    markedCount = result?.MarkedCount ?? 0,
                    cusId = cusId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for customer {CusId}", cusId);
                
                return Json(new 
                { 
                    success = false, 
                    message = "Lỗi khi đánh dấu tất cả thông báo đã đọc",
                    markedCount = 0,
                    cusId = cusId
                });
            }
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        [HttpGet("GetUnreadCount/{cusId}")]
        public async Task<IActionResult> GetUnreadCount(int cusId)
        {
            try
            {
                if (cusId <= 0)
                {
                    return BadRequest(new { message = "Customer ID không hợp lệ" });
                }

                var result = await _apiService.GetAsync<UnreadCountResponse>($"Notification/unread-count/{cusId}");
                
                return Json(new 
                { 
                    success = true,
                    unreadCount = result?.UnreadCount ?? 0,
                    cusId = cusId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for customer {CusId}", cusId);
                
                return Json(new 
                { 
                    success = false,
                    unreadCount = 0,
                    cusId = cusId
                });
            }
        }
    }

    // 👇 DTO classes
    public class MarkAsReadRequest
    {
        public int NotiId { get; set; }
        public int CusId { get; set; }
    }

    public class MarkAllAsReadResponse
    {
        public int MarkedCount { get; set; }
    }

    public class UnreadCountResponse
    {
        public int UnreadCount { get; set; }
    }
}