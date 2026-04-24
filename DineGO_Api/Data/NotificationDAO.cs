using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class NotificationDAO
    {
        private readonly ApplicationDbContext _context;

        public NotificationDAO(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Notification> AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.noti_id == id);
        }

        public async Task<bool> UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.noti_id == id);
            if (notification == null) return false;
            _context.Notifications.Remove(notification);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Notification>> GetNotificationsByCustomerIdAsync(int cusId)
        {
            var notifications = await _context.NotificationCustomers
                .Where(nc => nc.cus_id == cusId)
                .Include(nc => nc.notification)
                .OrderByDescending(nc => nc.noti_customer_send_date)
                .Select(nc => new Notification
                {
                    noti_id = nc.notification.noti_id,
                    noti_title = nc.notification.noti_title,
                    noti_content = nc.notification.noti_content,
                    noti_action = nc.notification.noti_action,
                    noti_type = nc.notification.noti_type,
                    // 👇 FIXED: Map từ NotificationCustomer thay vì Notification
                    noti_is_read = nc.noti_customer_is_read, // 👈 Từ NotificationCustomer
                    noti_date = nc.noti_customer_send_date
                })
                .ToListAsync();

            return notifications;
        }
        // 👇 NEW METHODS for Mark as Read functionality
        public async Task<bool> MarkAsReadAsync(int notiId, int cusId)
        {
            try
            {
                var notificationCustomer = await _context.NotificationCustomers
                    .FirstOrDefaultAsync(nc => nc.noti_id == notiId && nc.cus_id == cusId);
                
                if (notificationCustomer != null && !notificationCustomer.noti_customer_is_read)
                {
                    notificationCustomer.noti_customer_is_read = true;
                    notificationCustomer.read_date = DateTime.Now;

                    await _context.SaveChangesAsync();
                    return true;
                }

                return false; // Không tìm thấy hoặc đã đọc rồi
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> MarkAllAsReadAsync(int cusId)
        {
            try
            {
                var unreadNotifications = await _context.NotificationCustomers
                    .Where(nc => nc.cus_id == cusId && !nc.noti_customer_is_read)
                    .ToListAsync();

                if (unreadNotifications.Any())
                {
                    foreach (var notification in unreadNotifications)
                    {
                        notification.noti_customer_is_read = true;
                        notification.read_date = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                }

                return unreadNotifications.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> GetUnreadCountAsync(int cusId)
        {
            try
            {
                return await _context.NotificationCustomers
                    .CountAsync(nc => nc.cus_id == cusId && !nc.noti_customer_is_read);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // 👇 Method để check notification có thuộc về customer không
        public async Task<bool> IsNotificationOwnedByCustomerAsync(int notiId, int cusId)
        {
            try
            {
                return await _context.NotificationCustomers
                    .AnyAsync(nc => nc.noti_id == notiId && nc.cus_id == cusId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // 👇 Method để lấy chi tiết notification với trạng thái read
        public async Task<NotificationCustomerDetail> GetNotificationDetailAsync(int notiId, int cusId)
        {
            try
            {
                var result = await _context.NotificationCustomers
                    .Where(nc => nc.noti_id == notiId && nc.cus_id == cusId)
                    .Include(nc => nc.notification)
                    .Select(nc => new NotificationCustomerDetail
                    {
                        noti_id = nc.notification.noti_id,
                        noti_title = nc.notification.noti_title,
                        noti_content = nc.notification.noti_content,
                        noti_action = nc.notification.noti_action,
                        noti_type = nc.notification.noti_type,
                        noti_date = nc.noti_customer_send_date,
                        noti_is_read = nc.noti_customer_is_read,
                        read_date = nc.read_date,
                        cus_id = nc.cus_id
                    })
                    .FirstOrDefaultAsync();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public class NotificationCustomerDetail
    {
        public int noti_id { get; set; }
        public string noti_title { get; set; }
        public string noti_content { get; set; }
        public string noti_action { get; set; }
        public string noti_type { get; set; }
        public DateTime noti_date { get; set; }
        public bool noti_is_read { get; set; }
        public DateTime? read_date { get; set; }
        public int cus_id { get; set; }
    }
}