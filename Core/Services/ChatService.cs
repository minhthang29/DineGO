using Core.Models;
using Core.Common;
using Core.Constant;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

public class ChatService
{
    private readonly ApiService _api;
    private readonly IHttpContextAccessor _context;

    public ChatService(ApiService api, IHttpContextAccessor context)
    {
        _api = api;
        _context = context;
    }

    public async Task<bool> SendMessageAsync(int senderId, int receiverId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return false;

        var chat = new ChatMessage
        {
            sender_id = senderId,
            receiver_id = receiverId,
            message = message
        };

        try
        {
            await _api.PostAsync<object, ChatMessage>(ApiEndpoints.SEND, chat);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<Customer>> GetFriendListAsync(int customerId)
    {
        var url = ApiEndpoints.FRIEND_LIST + customerId;
        var friends = await _api.GetAsync<List<Customer>>(url);
        return friends ?? new List<Customer>();
    }

    public async Task<List<dynamic>> GetChatHistoryAsync(int senderId, int receiverId)
    {
        var url = ApiEndpoints.CHAT_HISTORY + $"?senderId={senderId}&receiverId={receiverId}";
        return await _api.GetAsync<List<dynamic>>(url) ?? new List<dynamic>();
    }

    public async Task<bool> SendFriendRequestAsync(int senderId, int targetId)
    {
        var payload = new Friend
        {
            customer_id = senderId,
            friend_customer_id = targetId
        };

        try
        {
            await _api.PostAsync<object, Friend>(ApiEndpoints.FRIEND_REQUEST, payload);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<bool> AcceptFriendRequestAsync(int currentId, int otherId)
    {
        var payload = new Friend
        {
            customer_id = currentId,          // B
            friend_customer_id = otherId      // A
        };

        await _api.PostAsync<object, Friend>(ApiEndpoints.FRIEND_ACCEPT, payload);
        return true;
    }

    public async Task<List<Customer>> SearchUsersAsync(string keyword, int currentUserId)
    {
        var url = $"{ApiEndpoints.FRIEND_SEARCH}?keyword={Uri.EscapeDataString(keyword)}&currentId={currentUserId}";
        var result = await _api.GetAsync<List<Customer>>(url);
        return result ?? new List<Customer>();
    }
    public async Task<bool> AreFriendsAsync(int a, int b)
    {
        var url = $"{ApiEndpoints.CHECK_FRIEND}?a={a}&b={b}";
        return await _api.GetAsync<bool>(url);
    }
    public async Task MarkMessagesAsReadAsync(int readerId, int senderId, bool isResOwnerChat)
    {
        var url = $"{ApiEndpoints.CHAT_MARK_READ}?readerId={readerId}&senderId={senderId}&isResOwnerChat={isResOwnerChat.ToString().ToLower()}";
        await _api.PostAsync<object, object>(url, new { }); // body có thể là {} hoặc new { foo = 1 }
    }

    public async Task<int> CountUnreadMessages(int senderId, int receiverId)
    {
        var url = $"{ApiEndpoints.CHAT_UNREAD_COUNT}?senderId={senderId}&receiverId={receiverId}";
        return await _api.GetAsync<int>(url);
    }

    public async Task<bool> SendMessageToResOwnerAsync(int senderId, int resOwnerId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return false;

        var chat = new ChatMessage
        {
            sender_id = senderId,
            receiver_id = resOwnerId,
            message = message,
            is_resowner_chat = true
        };

        try
        {
            await _api.PostAsync<object, ChatMessage>(ApiEndpoints.SEND, chat);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<List<dynamic>> GetChatHistoryAsync(int senderId, int receiverId, bool isResOwnerChat)
    {
        var url = $"{ApiEndpoints.CHAT_HISTORY}?senderId={senderId}&receiverId={receiverId}&isResOwnerChat={isResOwnerChat.ToString().ToLower()}";
        return await _api.GetAsync<List<dynamic>>(url) ?? new List<dynamic>();
    }
    public async Task<bool> SendFriendRequestToResOwnerAsync(int senderId, int resOwnerId)
    {
        var payload = new Friend
        {
            customer_id = senderId,
            friend_customer_id = resOwnerId
        };

        try
        {
            await _api.PostAsync<object, Friend>(ApiEndpoints.FRIEND_REQUEST_RESOWNER, payload);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<List<RestaurantOwner>> GetFollowedResOwnersAsync(int customerId)
    {
        var url = $"{ApiEndpoints.RESOWNER_FOLLOWER}?cusId={customerId}";
        return await _api.GetAsync<List<RestaurantOwner>>(url) ?? new List<RestaurantOwner>();
    }
    public async Task<List<Customer>> GetCustomersChattedWithResOwnerAsync(int resOwnerId)
    {
        var url = $"{ApiEndpoints.RESOWNER_FRIENDS}?resOwnerId={resOwnerId}";
        return await _api.GetAsync<List<Customer>>(url) ?? new List<Customer>();
    }
    public async Task<List<Customer>> GetFollowersOfResOwnerAsync(int resOwnerId)
    {
        var url = $"{ApiEndpoints.RESOWNER_CUSTOMER_FOLLOWERS}?resOwnerId={resOwnerId}";
        return await _api.GetAsync<List<Customer>>(url) ?? new List<Customer>();
    }
    // 👤 Lấy tất cả nhà hàng từ các ResOwner mà customer follow
    public async Task<List<Restaurant>> GetRestaurantsFollowedByCustomerAsync(int cusId)
    {
        var url = $"{ApiEndpoints.RESTAURANTS_BY_CUSTOMER_FOLLOWER}?cusId={cusId}";
        return await _api.GetAsync<List<Restaurant>>(url) ?? new List<Restaurant>();
    }

    // 🏪 Lấy danh sách nhà hàng của ResOwner + khách hàng đã follow ResOwner
    public async Task<List<dynamic>> GetRestaurantsWithFollowersAsync(int resOwnerId)
    {
        var url = $"{ApiEndpoints.RESTAURANTS_AND_FOLLOWERS_BY_RESOWNER}?resOwnerId={resOwnerId}";
        return await _api.GetAsync<List<dynamic>>(url) ?? new List<dynamic>();
    }

    public async Task<bool> SendMessageToRestaurantAsync(int senderCusId, int resId, string message)
    {
        var chat = new ChatMessage
        {
            sender_id = senderCusId,
            receiver_id = resId,
            message = message,
            is_resowner_chat = true
        };

        try
        {
            await _api.PostAsync<object, ChatMessage>(ApiEndpoints.SEND, chat);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int?> GetResOwnerIdByResIdAsync(int resId)
    {
        var url = ApiEndpoints.GET_RESOWNER_ID_BY_RESID + resId;
        var result = await _api.GetAsync<RestaurantOwnerResponse>(url);
        return result?.res_owner_id;
    }
}
