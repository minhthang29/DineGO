using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface INotificationRepository
    {
        Task<Notification> AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetAllNotificationsAsync();
        Task<Notification> GetNotificationByIdAsync(int id);
        Task<bool> UpdateNotificationAsync(Notification notification);
        Task<bool> DeleteNotificationAsync(int id);
        Task<List<Notification>> GetNotificationsByCustomerIdAsync(int cusId);
        Task<bool> MarkAsReadAsync(int notiId, int cusId);
        Task<int> MarkAllAsReadAsync(int cusId);
        Task<int> GetUnreadCountAsync(int cusId);
    }
}