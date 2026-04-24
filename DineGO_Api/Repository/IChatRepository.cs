using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IChatRepository
    {
        Task AddMessageAsync(int senderId, int receiverId, string message);
        /// <summary>
        /// Trả về toàn bộ tin nhắn 2 chiều giữa 2 user
        /// </summary>
        Task<List<ChatMessage>> GetMessagesAsync(int senderId, int receiverId);

        // 🆕 Bổ sung cho chấm đỏ
        Task MarkMessagesAsReadAsync(int readerId, int senderId);
        Task<int> CountUnreadMessagesAsync(int senderId, int receiverId);

        Task AddMessageWithResOwnerAsync(int senderId, int resOwnerId, string message);
        Task<List<ChatMessage>> GetChatHistoryAsync(int senderId, int receiverId, bool isResOwnerChat);
        Task AddMessageToRestaurantAsync(int senderCusId, int resId, string message);
    }
}