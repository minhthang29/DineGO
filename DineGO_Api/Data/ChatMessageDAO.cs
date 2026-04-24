using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class ChatMessageDAO
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            message.is_read = false;
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(int userA, int userB)
        {
            return await _context.ChatMessages
            .Where(m => (m.sender_id == userA && m.receiver_id == userB)
                    || (m.sender_id == userB && m.receiver_id == userA))
            .OrderBy(m => m.sent_at)
            .ToListAsync();
        }

        public async Task MarkMessagesAsReadAsync(int readerId, int senderId)
        {
            var unreadMessages = await _context.ChatMessages
                .Where(m => m.sender_id == senderId && m.receiver_id == readerId && !m.is_read)
                .ToListAsync();

            foreach (var msg in unreadMessages)
            {
                msg.is_read = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountUnreadAsync(int senderId, int receiverId)
        {
            return await _context.ChatMessages
                .Where(m => m.sender_id == senderId && m.receiver_id == receiverId && !m.is_read)
                .CountAsync();
        }
        public async Task<List<ChatMessage>> GetChatHistoryAsync(int senderId, int receiverId, bool isResOwnerChat)
        {
            return await _context.ChatMessages
                .Where(m =>
                    m.is_resowner_chat == isResOwnerChat &&
                    ((m.sender_id == senderId && m.receiver_id == receiverId) ||
                     (m.sender_id == receiverId && m.receiver_id == senderId))
                )
                .OrderBy(m => m.sent_at)
                .ToListAsync();
        }
        public async Task AddMessageWithResOwnerAsync(int senderId, int resOwnerId, string message)
        {
            var msg = new ChatMessage
            {
                sender_id = senderId,
                receiver_id = resOwnerId,
                message = message,
                is_read = false,
                is_resowner_chat = true,
                sent_at = DateTime.UtcNow
            };

            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
        }
        public async Task AddMessageToRestaurantAsync(int senderCusId, int resId, string message)
        {
            var msg = new ChatMessage
            {
                sender_id = senderCusId,
                receiver_id = resId,
                message = message,
                is_read = false,
                is_resowner_chat = true,
                sent_at = DateTime.UtcNow
            };
            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();
        }
    }
}