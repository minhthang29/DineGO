using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Core.Services;
using System.Collections.Generic;
using System.Linq;
using System;
using Core.Constant;
using System.Text.Json;
using System.Collections.Concurrent;
using Core.Models;


namespace DineGO_Client.SignalR
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<int, string> OnlineUsers = new();
        private readonly ChatService _chatService;
        private readonly AIService _aiService;

        public ChatHub(ChatService chatService, AIService aIService)
        {
            _chatService = chatService;
            _aiService = aIService;
        }

        public async Task SendMessage(int receiverId, string message)
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "Bạn chưa đăng nhập!");
                return;
            }
            // 👉 GỌI 1 LẦN DUY NHẤT
            bool areFriends = await _chatService.AreFriendsAsync(senderId.Value, receiverId);
            if (!areFriends)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", 0, "❌ Hai người chưa kết bạn.");
                return;
            }


            var success = await _chatService.SendMessageAsync(senderId.Value, receiverId, message);
            if (!success)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "Gửi tin nhắn thất bại!");
                return;
            }

            // ✅ Gọi API lấy lịch sử mới nhất giữa 2 người
            var messages = await _chatService.GetChatHistoryAsync(senderId.Value, receiverId);

            // ✅ Gửi lịch sử về cho cả người gửi và người nhận
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId.Value, message, false);
            await Clients.User(senderId.ToString()).SendAsync("ReceiveChatHistoryCustomer", messages); // ✅ đổi event
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveChatHistoryCustomer", messages); // ✅ đổi event
        }
        public async Task LoadFriendList()
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveFriendList", new List<object>());
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "Không tìm thấy phiên đăng nhập!");
                return;
            }

            var friends = await _chatService.GetFriendListAsync(senderId.Value);
            var result = new List<object>();

            foreach (var friend in friends)
            {
                var history = await _chatService.GetChatHistoryAsync(senderId.Value, friend.cus_id); // List<dynamic>
                int unreadCount = await _chatService.CountUnreadMessages(friend.cus_id, senderId.Value); // từ A gửi cho mình

                string lastMsg = "Chưa có tin nhắn";
                var isOnline = OnlineUsers.ContainsKey(friend.cus_id);

                try
                {
                    if (history != null && history.Count > 0)
                    {
                        var last = history.Last();

                        string msg = "Không rõ";
                        if (last.TryGetProperty("message", out JsonElement prop))
                        {
                            msg = prop.GetString() ?? "Không rõ";
                        }

                        lastMsg = msg;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Lỗi lấy msg cuối của {friend.cus_name}: {ex.Message}");
                }


                result.Add(new
                {
                    cus_id = friend.cus_id,
                    cus_name = friend.cus_name,
                    cus_image = string.IsNullOrEmpty(friend.cus_image) ? null : friend.cus_image,
                    last_message = lastMsg,
                    is_online = isOnline,
                    has_unread = unreadCount > 0
                });
            }
            await Clients.Caller.SendAsync("ReceiveFriendList", result);
        }


        public async Task LoadChatHistory(int friendId)
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveChatHistoryCustomer", new
                {
                    messages = new List<object>(),
                    is_pending = false
                });
                return;
            }

            var messages = await _chatService.GetChatHistoryAsync(senderId.Value, friendId);

            try
            {
                await _chatService.MarkMessagesAsReadAsync(senderId.Value, friendId, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Mark as read failed: " + ex.Message);
            }

            var isAccepted = await _chatService.AreFriendsAsync(friendId, senderId.Value);

            await Clients.Caller.SendAsync("ReceiveChatHistoryCustomer", new
            {
                messages = messages,
                is_pending = !isAccepted
            });

        }

        public async Task AcceptFriend(int friendId)
        {
            var senderId = GetSenderId(); // người bấm chấp nhận
            await _chatService.AcceptFriendRequestAsync(senderId.Value, friendId); // B → A
        }
        public async Task SendFriendRequest(int friendId)
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "Không xác định người gửi.");
                return;
            }

            var success = await _chatService.SendFriendRequestAsync(senderId.Value, friendId);
            if (success)
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "✅ Đã gửi lời mời kết bạn.");
            else
                await Clients.Caller.SendAsync("ReceiveMessage", "Hệ thống", "❌ Gửi lời mời thất bại hoặc đã gửi trước đó.");
        }

        public async Task SearchUsers(string keyword)
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveSearchResult", new List<object>());
                return;
            }

            var friends = await _chatService.GetFriendListAsync(senderId.Value);
            var friendIds = friends.Select(f => f.cus_id).ToHashSet();

            var allMatches = await _chatService.SearchUsersAsync(keyword, senderId.Value);

            var filtered = allMatches
                .Where(u => !friendIds.Contains(u.cus_id) && u.cus_id != senderId) // lọc ra người CHƯA kết bạn, và không phải chính mình
                .Select(u => new
                {
                    cus_id = u.cus_id,
                    cus_name = u.cus_name,
                    cus_username = u.cus_username,
                    cus_image = string.IsNullOrEmpty(u.cus_image)
                        ? "https://cdn-icons-png.flaticon.com/512/149/149071.png"
                        : u.cus_image
                }).ToList();

            await Clients.Caller.SendAsync("ReceiveSearchResult", filtered);
        }

        private int? GetSenderId()
        {
            var httpContext = Context.GetHttpContext();
            var id = httpContext?.Session?.GetInt32(SessionConstants.CUSTOMER_ID);
            return id;
        }

        public override async Task OnConnectedAsync()
        {
            var senderId = GetSenderId();
            if (senderId != null)
            {
                OnlineUsers[senderId.Value] = Context.ConnectionId;

                // 🔴 Gửi trạng thái online cho bạn bè
                var friends = await _chatService.GetFriendListAsync(senderId.Value);
                foreach (var friend in friends)
                {
                    if (OnlineUsers.TryGetValue(friend.cus_id, out var connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("FriendOnline", senderId.Value);
                    }
                }

                // 🔴 Gửi trạng thái online cho ResOwner mà customer này đang theo dõi
                var followedResOwners = await _chatService.GetFollowedResOwnersAsync(senderId.Value);
                foreach (var res in followedResOwners)
                {
                    if (OnlineUsers.TryGetValue(res.res_owner_id, out var connId))
                    {
                        await Clients.Client(connId).SendAsync("FriendOnline", senderId.Value);
                    }
                }

                await Clients.Caller.SendAsync("SetCurrentUserId", senderId.Value);
            }

            var resOwnerId = GetResOwnerId();
            if (resOwnerId != null)
            {
                OnlineUsers[-resOwnerId.Value] = Context.ConnectionId; // 🟢 Dùng số âm để phân biệt ResOwner
                await Clients.Caller.SendAsync("SetCurrentResOwnerId", resOwnerId.Value);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var senderId = GetSenderId();
            if (senderId != null)
            {
                OnlineUsers.TryRemove(senderId.Value, out _);

                // 🔴 Gửi trạng thái offline cho bạn bè
                var friends = await _chatService.GetFriendListAsync(senderId.Value);
                foreach (var friend in friends)
                {
                    if (OnlineUsers.TryGetValue(friend.cus_id, out var connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("FriendOffline", senderId.Value);
                    }
                }

                // 🔴 Gửi trạng thái offline cho ResOwner mà customer này đang theo dõi
                var followedResOwners = await _chatService.GetFollowedResOwnersAsync(senderId.Value);
                foreach (var res in followedResOwners)
                {
                    if (OnlineUsers.TryGetValue(res.res_owner_id, out var connId))
                    {
                        await Clients.Client(connId).SendAsync("FriendOffline", senderId.Value);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CallUser(int targetUserId, string offer)
        {
            var callerId = GetSenderId();
            if (callerId != null && OnlineUsers.TryGetValue(targetUserId, out var connId))
            {
                await Clients.Client(connId).SendAsync("ReceiveCall", callerId.Value, offer);
            }
        }

        public async Task AnswerCall(int callerId, string answer)
        {
            var receiverId = GetSenderId();
            if (receiverId != null && OnlineUsers.TryGetValue(callerId, out var connId))
            {
                await Clients.Client(connId).SendAsync("ReceiveAnswer", receiverId.Value, answer);
            }
        }

        public async Task SendIceCandidate(int targetUserId, string candidate)
        {
            var senderId = GetSenderId();
            if (senderId != null && OnlineUsers.TryGetValue(targetUserId, out var connId))
            {
                await Clients.Client(connId).SendAsync("ReceiveIceCandidate", senderId.Value, candidate);
            }
        }

        public async Task RejectCall(int targetUserId)
        {
            var senderId = GetSenderId();
            if (senderId != null && OnlineUsers.TryGetValue(targetUserId, out var connId))
            {
                await Clients.Client(connId).SendAsync("CallRejected", senderId.Value);
            }
        }
        public async Task EndCall(int targetUserId)
        {
            var senderId = GetSenderId();
            if (senderId != null && OnlineUsers.TryGetValue(targetUserId, out var connId))
            {
                await Clients.Client(connId).SendAsync("CallEnded", senderId.Value);
            }
        }

        public async Task SendAIMessage(string userMessage)
        {
            var result = await _aiService.GetFullSuggestionAsync(userMessage);
            await Clients.Caller.SendAsync("ReceiveAISuggestion", result);
        }
        public async Task LoadResOwnerList()
        {
            var senderId = GetSenderId();
            if (senderId == null) return;

            var restaurants = await _chatService.GetRestaurantsFollowedByCustomerAsync(senderId.Value);
            var result = new List<object>();

            foreach (var res in restaurants)
            {
                var messages = await _chatService.GetChatHistoryAsync(senderId.Value, res.res_id, true);
                var unread = await _chatService.CountUnreadMessages(res.res_id, senderId.Value);

                string rawMsg = messages.LastOrDefault()?.GetProperty("message")?.GetString() ?? "Chưa có tin nhắn";
                string cleanMsg = rawMsg.StartsWith("ro:") ? rawMsg.Substring(3) : rawMsg;

                result.Add(new
                {
                    res_id = res.res_id,
                    res_name = res.res_name,
                    last_message = cleanMsg,
                    has_unread = unread > 0
                });
            }

            await Clients.Caller.SendAsync("ReceiveResOwnerList", result);
        }
        public async Task SendMessageToResOwner(int resId, string message)
        {
            var senderId = GetSenderId();
            if (senderId == null) return;

            await _chatService.SendMessageToRestaurantAsync(senderId.Value, resId, message);

            var messages = await _chatService.GetChatHistoryAsync(senderId.Value, resId, true);
            await Clients.Caller.SendAsync("ReceiveChatHistoryResOwner", new { messages });

            // ✅ Lấy res_owner_id từ ChatService
            var actualResOwnerId = await _chatService.GetResOwnerIdByResIdAsync(resId);
            if (actualResOwnerId != null)
            {
                if (OnlineUsers.TryGetValue(-actualResOwnerId.Value, out var roConn))
                {
                    await Clients.Client(roConn).SendAsync("ReceiveMessage", senderId.Value, message, true);
                }
            }

            // 🟢 Gửi về cho chính mình (Customer)
            if (OnlineUsers.TryGetValue(senderId.Value, out var cusConn))
            {
                await Clients.Client(cusConn).SendAsync("ReceiveMessage", senderId.Value, message, true);
            }
        }

        public async Task LoadChatHistoryWithResOwner(int resId)
        {
            var senderId = GetSenderId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("ReceiveChatHistoryResOwner", new { messages = new List<object>() });
                return;
            }

            var messages = await _chatService.GetChatHistoryAsync(senderId.Value, resId, true);

            if (messages == null)
                messages = new List<object>();

            await _chatService.MarkMessagesAsReadAsync(senderId.Value, resId, true);

            await Clients.Caller.SendAsync("ReceiveChatHistoryResOwner", new { messages });
        }

        public async Task LoadMoreChatHistory(int friendId, int offset)
        {
            var senderId = GetSenderId();
            if (senderId == null) return;

            var allMessages = await _chatService.GetChatHistoryAsync(senderId.Value, friendId);
            var paged = allMessages
                .AsEnumerable()
                .Reverse()
                .Skip(offset)
                .Take(10)
                .ToList();

            await Clients.Caller.SendAsync("ReceiveMoreChatHistory", paged);
        }

        public async Task LoadMoreChatHistoryResOwner(int resOwnerId, int offset)
        {
            var senderId = GetSenderId();
            if (senderId == null) return;

            var messages = await _chatService.GetChatHistoryAsync(senderId.Value, resOwnerId, true);
            var paged = messages
                .AsEnumerable()
                .Reverse()
                .Skip(offset)
                .Take(10)
                .ToList();

            await Clients.Caller.SendAsync("ReceiveMoreChatHistoryResOwner", paged);
        }
        public async Task SendMessageFromResOwner(int customerId, string message, int resId)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            await _chatService.SendMessageToRestaurantAsync(resId, customerId, "ro:" + message);
            var messages = await _chatService.GetChatHistoryAsync(resId, customerId, true);
            await Clients.Caller.SendAsync("ReceiveChatHistoryResOwner", new { messages });

            var resOwnerId = GetResOwnerId();
            if (resOwnerId == null) return;

            // 🟢 Gửi cho Customer nếu online
            if (OnlineUsers.TryGetValue(customerId, out var cusConn))
            {
                await Clients.Client(cusConn).SendAsync("ReceiveMessage", resId, "ro:" + message, true);
            }
        }

        public async Task LoadChatHistoryFromResOwner(int customerId, int resId)
        {
            var messages = await _chatService.GetChatHistoryAsync(resId, customerId, true);
            await _chatService.MarkMessagesAsReadAsync(resId, customerId, true);
            await Clients.Caller.SendAsync("ReceiveChatHistoryResOwner", new
            {
                messages = messages,
                res_id = resId,
                resowner_id = GetResOwnerId()
            });
        }

        public async Task LoadMoreChatHistoryFromResOwner(int customerId, int resId, int offset)
        {
            var all = await _chatService.GetChatHistoryAsync(resId, customerId, true);
            var paged = all.AsEnumerable().Reverse().Skip(offset).Take(10).ToList();

            await Clients.Caller.SendAsync("ReceiveMoreChatHistoryResOwner", new
            {
                messages = paged,
                res_id = resId,
                resowner_id = GetResOwnerId()
            });
        }

        public async Task LoadResOwnerRestaurantsWithFollowers()
        {
            var resOwnerId = GetResOwnerId();

            if (resOwnerId == null)
            {
                return;
            }

            // ✅ Ép kiểu chắc chắn
            List<dynamic> rawData = await _chatService.GetRestaurantsWithFollowersAsync(resOwnerId.Value);;

            var result = new List<object>();

            foreach (JsonElement item in rawData)
            {
                try
                {
                    var restaurant = item.GetProperty("restaurant");
                    var followers = item.GetProperty("followers");

                    var res_id = restaurant.GetProperty("res_id").GetInt32();
                    var res_name = restaurant.GetProperty("res_name").GetString();

                    var followerList = new List<object>();
                    foreach (JsonElement c in followers.EnumerateArray())
                    {
                        var cusId = c.GetProperty("cus_id").GetInt32();
                        var lastMsgs = await _chatService.GetChatHistoryAsync(res_id, cusId, true);
                        var lastMsgRaw = lastMsgs.LastOrDefault()?.GetProperty("message")?.GetString() ?? "Chưa có tin nhắn";
                        var lastMsg = lastMsgRaw.StartsWith("ro:") ? lastMsgRaw.Substring(3) : lastMsgRaw;
                        var unread = await _chatService.CountUnreadMessages(cusId, res_id);
                        var isOnline = OnlineUsers.ContainsKey(cusId);

                        followerList.Add(new
                        {
                            cus_id = cusId,
                            cus_name = c.GetProperty("cus_name").GetString(),
                            cus_image = c.GetProperty("cus_image").GetString(),
                            last_message = lastMsg,
                            is_online = isOnline,
                            has_unread = unread > 0
                        });
                    }

                    result.Add(new
                    {
                        res_id = res_id,
                        res_name = res_name,
                        followers = followerList
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Parse restaurant item failed: " + ex.Message);
                }
            }

            await Clients.Caller.SendAsync("ReceiveRestaurantsWithFollowers", result);
        }

        private int? GetResOwnerId()
        {
            var httpContext = Context.GetHttpContext();
            var id = httpContext?.Session?.GetInt32(SessionConstants.RESTAURANT_OWNER_ID);
            return id;
        }

    }
}