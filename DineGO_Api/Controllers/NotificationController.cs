using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DineGO_Api.Repository;
using Core.Models;
using System;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        // GET: api/notification
        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetAll()
        {
            var notifications = await _notificationRepository.GetAllNotificationsAsync();
            return Ok(notifications);
        }

        // GET: api/notification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetById(int id)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);
            if (notification == null) return NotFound();
            return Ok(notification);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<List<Notification>>> GetByCustomerId(int customerId)
        {
            var notifications = await _notificationRepository.GetNotificationsByCustomerIdAsync(customerId);
            return Ok(notifications);
        }

        // POST: api/notification
        [HttpPost]
        public async Task<ActionResult<Notification>> Create([FromBody] Notification notification)
        {
            var created = await _notificationRepository.AddNotificationAsync(notification);
            return CreatedAtAction(nameof(GetById), new { id = created.noti_id }, created);
        }

        // PUT: api/notification/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Notification notification)
        {
            if (id != notification.noti_id) return BadRequest();
            var result = await _notificationRepository.UpdateNotificationAsync(notification);
            if (!result) return NotFound();
            return Ok(new { noti_id = id });
        }

        // DELETE: api/notification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _notificationRepository.DeleteNotificationAsync(id);
            if (!result) return NotFound();
            return Ok(new { noti_id = id });
        }
        // 👇 NEW API: POST: api/notification/mark-as-read
        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
        {
            try
            {
                if (request == null || request.NotiId <= 0 || request.CusId <= 0)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Thông tin không hợp lệ. Vui lòng kiểm tra notiId và cusId." 
                    });
                }

                var result = await _notificationRepository.MarkAsReadAsync(request.NotiId, request.CusId);
                
                if (result)
                {
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Đánh dấu đã đọc thành công",
                        notiId = request.NotiId,
                        cusId = request.CusId
                    });
                }
                else
                {
                    return NotFound(new 
                    { 
                        success = false, 
                        message = "Không tìm thấy thông báo hoặc thông báo không thuộc về khách hàng này" 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Lỗi hệ thống khi đánh dấu đã đọc",
                    error = ex.Message
                });
            }
        }

        // 👇 NEW API: POST: api/notification/mark-all-as-read/{customerId}
        [HttpPost("mark-all-as-read/{customerId}")]
        public async Task<IActionResult> MarkAllAsRead(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Customer ID không hợp lệ" 
                    });
                }

                var result = await _notificationRepository.MarkAllAsReadAsync(customerId);
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Đánh dấu tất cả thông báo đã đọc thành công",
                    markedCount = result,
                    cusId = customerId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Lỗi hệ thống khi đánh dấu tất cả đã đọc",
                    error = ex.Message
                });
            }
        }

        // 👇 NEW API: GET: api/notification/unread-count/{customerId}
        [HttpGet("unread-count/{customerId}")]
        public async Task<ActionResult<int>> GetUnreadCount(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    return BadRequest(new { message = "Customer ID không hợp lệ" });
                }

                var count = await _notificationRepository.GetUnreadCountAsync(customerId);
                
                return Ok(new 
                { 
                    success = true,
                    unreadCount = count,
                    cusId = customerId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = "Lỗi hệ thống khi lấy số thông báo chưa đọc",
                    error = ex.Message
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
}