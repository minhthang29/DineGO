using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IFriendRepository
    {
        Task<bool> AreFriendsAsync(int userId1, int userId2);
        Task<List<Customer>> GetFriendsAsync(int userId);
        Task<List<Customer>> SearchUsersAsync(string keyword, int currentUserId);
        Task AddFriendAsync(int userId, int targetId);
        Task AddFriendWithResOwnerAsync(int customerId, int resOwnerId);
        Task<List<RestaurantOwner>> GetFriendsByFollowerAsync(int cusId);
        Task<List<Customer>> GetFriendsByResOwnerAsync(int resOwnerId);
        Task<List<Customer>> GetCustomerFollowersAsync(int resOwnerId);
        Task<Dictionary<Restaurant, List<Customer>>> GetRestaurantsAndFollowersAsync(int resOwnerId);
        Task<List<Restaurant>> GetRestaurantsByCustomerFollowerAsync(int cusId);
        Task<int?> GetResOwnerIdByResIdAsync(int resId);
    }
}