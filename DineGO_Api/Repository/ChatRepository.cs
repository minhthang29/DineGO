using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatMessageDAO _chatDAO;

        public ChatRepository(ChatMessageDAO chatDAO)
        {
            _chatDAO = chatDAO;
        }

        public async Task AddMessageAsync(int senderId, int receiverId, string message)
        {
            var msg = new ChatMessage
            {
                sender_id = senderId,
                receiver_id = receiverId,
                message = message,
                sent_at = DateTime.UtcNow
            };

            await _chatDAO.AddMessageAsync(msg);
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(int senderId, int receiverId)
        {
            return await _chatDAO.GetMessagesAsync(senderId, receiverId);
        }

        public async Task MarkMessagesAsReadAsync(int readerId, int senderId)
        {
            await _chatDAO.MarkMessagesAsReadAsync(readerId, senderId);
        }

        public async Task<int> CountUnreadMessagesAsync(int senderId, int receiverId)
        {
            return await _chatDAO.CountUnreadAsync(senderId, receiverId);
        }
        public async Task AddMessageWithResOwnerAsync(int senderId, int resOwnerId, string message)
        {
            await _chatDAO.AddMessageWithResOwnerAsync(senderId, resOwnerId, message);
        }
        public async Task<List<ChatMessage>> GetChatHistoryAsync(int senderId, int receiverId, bool isResOwnerChat)
        {
            return await _chatDAO.GetChatHistoryAsync(senderId, receiverId, isResOwnerChat);
        }
        public async Task AddMessageToRestaurantAsync(int senderCusId, int resId, string message)
        {
            await _chatDAO.AddMessageToRestaurantAsync(senderCusId, resId, message);
        }

    }
}