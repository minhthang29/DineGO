using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.SignalRHub;
using Microsoft.AspNetCore.SignalR;

namespace DineGO_Api.Services
{
    public class NotificationService
    {
        private ApplicationDbContext _context;
        private IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Tạo notification mới, trả về notification vừa tạo
        public Notification CreateNotification(string title, string content, string type)
        {
            var notification = new Notification
            {
                noti_title = title,
                noti_content = content,
                noti_type = type,
                noti_date = DateTime.Now,
                noti_is_read = false,
                noti_is_deleted = false
            };
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            return notification;
        }

        // Gán notification cho customer
        public void SaveNotificationForCustomer(int notiId, int cusId, bool isRead = false)
        {
            var notificationCustomer = new NotificationCustomer
            {
                noti_id = notiId,
                cus_id = cusId,
                noti_customer_is_read = isRead,
                noti_customer_send_date = DateTime.Now
            };
            _context.NotificationCustomers.Add(notificationCustomer);
            _context.SaveChanges();
        }

        public void SendNotificationToCustomer(int cusId, string message, string title, string type)
        {
            _hubContext.Clients.User(cusId.ToString()).SendAsync("ReceiveNotification", new
            {
                title = title,
                content = message,
                type = type,
                date = DateTime.Now
            }).Wait();
        }

        public void NotifyCustomer(int cusId, string title, string content, string type, bool isRead = false)
        {
            // 1. Tạo notification
            var notification = CreateNotification(title, content, type);
            // 2. Gán notification cho customer
            SaveNotificationForCustomer(notification.noti_id, cusId, isRead);
            // 3. Gửi SignalR cho customer
            SendNotificationToCustomer(cusId, content, title, type);
        }
        public void NotifyAllUsers(string title, string content, string type)
        {
           // 1. Tạo notification
            var notification = CreateNotification(title, content, type);

            // Gán notification cho tất cả customer
            var allCustomers = _context.Customers.ToList();
            foreach (var customer in allCustomers)
            {
                var notificationCustomer = new NotificationCustomer
                {
                    noti_id = notification.noti_id,
                    cus_id = customer.cus_id,
                    noti_customer_is_read = false
                };
                _context.NotificationCustomers.Add(notificationCustomer);
            }
            _context.SaveChanges();

            // Gửi SignalR cho tất cả user
            _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                title = title,
                content = content,
                type = type,
                date = notification.noti_date
            }).Wait();
        }
    }
}