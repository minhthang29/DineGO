using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace DineGO_Api.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;

        public NotificationRepository(NotificationDAO notificationDAO)
        {
            _notificationDAO = notificationDAO;
        }
        public Task<Notification> AddNotificationAsync(Notification notification)
            => _notificationDAO.AddNotificationAsync(notification);

        public Task<List<Notification>> GetAllNotificationsAsync()
            => _notificationDAO.GetAllNotificationsAsync();

        public Task<Notification> GetNotificationByIdAsync(int id)
            => _notificationDAO.GetNotificationByIdAsync(id);

        public Task<bool> UpdateNotificationAsync(Notification notification)
            => _notificationDAO.UpdateNotificationAsync(notification);

        public Task<bool> DeleteNotificationAsync(int id)
            => _notificationDAO.DeleteNotificationAsync(id);
        public Task<List<Notification>> GetNotificationsByCustomerIdAsync(int cusId)
            => _notificationDAO.GetNotificationsByCustomerIdAsync(cusId);
        public async Task<bool> MarkAsReadAsync(int notiId, int cusId)
        {
            return await _notificationDAO.MarkAsReadAsync(notiId, cusId);
        }

        public async Task<int> MarkAllAsReadAsync(int cusId)
        {
            return await _notificationDAO.MarkAllAsReadAsync(cusId);
        }

        public async Task<int> GetUnreadCountAsync(int cusId)
        {
            return await _notificationDAO.GetUnreadCountAsync(cusId);
        }
    }
}