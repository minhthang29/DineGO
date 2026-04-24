using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Core.Models;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepo;
        private readonly IFriendRepository _friendRepo;

        public ChatController(IChatRepository chatRepo, IFriendRepository friendRepo)
        {
            _chatRepo = chatRepo;
            _friendRepo = friendRepo;
        }

        // POST: api/chat/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.message))
                return BadRequest(new { error = "Tin nhắn không được để trống" });

            message.sent_at = DateTime.UtcNow;

            if (message.is_resowner_chat)
            {
                // 💥 SỬA Ở ĐÂY: receiver_id là res_id, không còn là res_owner_id
                await _chatRepo.AddMessageToRestaurantAsync(message.sender_id, message.receiver_id, message.message);
            }
            else
            {
                await _chatRepo.AddMessageAsync(message.sender_id, message.receiver_id, message.message);
            }

            return Ok(new { success = true });
        }

        [HttpGet("friend-list")]
        public async Task<IActionResult> GetFriendList([FromQuery] int cusId)
        {
            var friends = await _friendRepo.GetFriendsAsync(cusId);
            return Ok(friends);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] int senderId, [FromQuery] int receiverId, [FromQuery] bool isResOwnerChat)
        {
            var messages = await _chatRepo.GetChatHistoryAsync(senderId, receiverId, isResOwnerChat);

            var result = messages.Select(m => new
            {
                sender_id = m.sender_id,
                message = m.message,
                sent_at = m.sent_at.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return Ok(result);
        }

        // POST: api/friend/request
        [HttpPost("request")]
        public async Task<IActionResult> SendFriendRequest([FromBody] Friend request)
        {
            // Kiểm tra nếu đã tồn tại (user → friend)
            var alreadyExists = await _friendRepo.AreFriendsAsync(request.customer_id, request.friend_customer_id);
            if (alreadyExists)
                return BadRequest(new { message = "Đã gửi lời mời hoặc đã là bạn bè." });

            // Tạo lời mời 1 chiều
            await _friendRepo.AddFriendAsync(request.customer_id, request.friend_customer_id);
            return Ok(new { message = "Đã gửi lời mời kết bạn!" });
        }

        // POST: api/friend/accept
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriend([FromBody] Friend accept)
        {
            await _friendRepo.AddFriendAsync(accept.customer_id, accept.friend_customer_id);
            return Ok(new { message = "Đã chấp nhận lời mời." });
        }

        // GET: api/friend/search?currentUserId=1&keyword=an
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword, [FromQuery] int currentId)
        {
            var result = await _friendRepo.SearchUsersAsync(keyword, currentId);
            return Ok(result);
        }

        [HttpGet("check-friend")]
        public async Task<IActionResult> CheckFriend(int a, int b)
        {
            bool areFriends = await _friendRepo.AreFriendsAsync(a, b); // ✅ Gọi đúng vào repository trung gian
            return Ok(areFriends);
        }

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromQuery] int readerId, [FromQuery] int senderId, [FromQuery] bool isResOwnerChat)
        {
            // Nếu bạn cần xử lý riêng cho ResOwner thì update ChatDAO
            await _chatRepo.MarkMessagesAsReadAsync(readerId, senderId); // hiện tại dùng chung
            return Ok(new { success = true });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> CountUnreadMessages([FromQuery] int senderId, [FromQuery] int receiverId)
        {
            int count = await _chatRepo.CountUnreadMessagesAsync(senderId, receiverId);
            return Ok(count);
        }

        [HttpGet("resowner-follower")]
        public async Task<IActionResult> GetResOwnerFollowers([FromQuery] int cusId)
        {
            var list = await _friendRepo.GetFriendsByFollowerAsync(cusId);
            return Ok(list);
        }
        [HttpGet("resowner-friends")]
        public async Task<IActionResult> GetFriendsOfResOwner([FromQuery] int resOwnerId)
        {
            var list = await _friendRepo.GetFriendsByResOwnerAsync(resOwnerId);
            return Ok(list);
        }
        [HttpPost("request-resowner")]
        public async Task<IActionResult> SendFriendRequestToResOwner([FromBody] Friend request)
        {
            if (request.customer_id <= 0 || request.friend_customer_id <= 0)
            {
                return BadRequest(new { message = "Thông tin không hợp lệ." });
            }

            await _friendRepo.AddFriendWithResOwnerAsync(request.customer_id, request.friend_customer_id);
            return Ok(new { message = "✅ Đã gửi lời mời kết bạn tới ResOwner!" });
        }
        [HttpGet("customer-followers")]
        public async Task<IActionResult> GetCustomerFollowers([FromQuery] int resOwnerId)
        {
            var list = await _friendRepo.GetCustomerFollowersAsync(resOwnerId);
            return Ok(list);
        }
        [HttpGet("restaurants-followed-by-customer")]
        public async Task<IActionResult> GetRestaurantsFollowedByCustomer([FromQuery] int cusId)
        {
            var list = await _friendRepo.GetRestaurantsByCustomerFollowerAsync(cusId);
            return Ok(list);
        }

        [HttpGet("resowner-restaurants-and-followers")]
        public async Task<IActionResult> GetRestaurantsAndFollowers([FromQuery] int resOwnerId)
        {
            var dict = await _friendRepo.GetRestaurantsAndFollowersAsync(resOwnerId);

            var result = dict.Select(kv => new
            {
                restaurant = new
                {
                    res_id = kv.Key.res_id,
                    res_name = kv.Key.res_name,
                    res_description = kv.Key.res_description
                },
                followers = kv.Value.Select(c => new
                {
                    cus_id = c.cus_id,
                    cus_name = c.cus_name,
                    cus_image = c.cus_image
                })
            });

            return Ok(result);
        }

        [HttpGet("restaurant/{resId}")]
        public async Task<IActionResult> GetResOwnerIdByRestaurant(int resId)
        {
            var resOwnerId = await _friendRepo.GetResOwnerIdByResIdAsync(resId);
            if (resOwnerId == null)
                return NotFound(new { message = "Không tìm thấy nhà hàng hoặc không có chủ." });

            return Ok(new
            {
                res_id = resId,
                res_owner_id = resOwnerId
            });
        }

    }
}