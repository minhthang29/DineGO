using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;

namespace DineGO_Api.Repository
{
    public class FriendRepository : IFriendRepository
    {
        private readonly FriendDAO _friendDAO;

        public FriendRepository(FriendDAO friendDAO)
        {
            _friendDAO = friendDAO;
        }

        public async Task<bool> AreFriendsAsync(int userId1, int userId2)
        {
            return await _friendDAO.AreFriendsAsync(userId1, userId2);
        }

        public async Task<List<Customer>> GetFriendsAsync(int userId)
        {
            return await _friendDAO.GetFriendsAsync(userId);
        }

        public async Task AddFriendAsync(int userId, int targetId)
        {
            var newFriend = new Friend
            {
                customer_id = userId,
                friend_customer_id = targetId,
                created_at = DateTime.UtcNow
            };
            await _friendDAO.AddFriendAsync(newFriend);
        }
        public async Task<List<Customer>> SearchUsersAsync(string keyword, int currentUserId)
        {
            return await _friendDAO.SearchUsersAsync(keyword, currentUserId);
        }
        public async Task AddFriendWithResOwnerAsync(int customerId, int resOwnerId)
        {
            await _friendDAO.AddFriendWithResOwnerAsync(customerId, resOwnerId);
        }
        public async Task<List<RestaurantOwner>> GetFriendsByFollowerAsync(int cusId)
        {
            return await _friendDAO.GetFriendsByFollowerAsync(cusId);
        }
        public async Task<List<Customer>> GetFriendsByResOwnerAsync(int resOwnerId)
        {
            return await _friendDAO.GetFriendsByResOwnerAsync(resOwnerId);
        }
        public async Task<List<Customer>> GetCustomerFollowersAsync(int resOwnerId)
        {
            return await _friendDAO.GetCustomerFollowersAsync(resOwnerId);
        }
        public async Task<List<Restaurant>> GetRestaurantsByCustomerFollowerAsync(int cusId)
        {
            return await _friendDAO.GetRestaurantsByCustomerFollowerAsync(cusId);
        }

        public async Task<Dictionary<Restaurant, List<Customer>>> GetRestaurantsAndFollowersAsync(int resOwnerId)
        {
            return await _friendDAO.GetRestaurantsAndFollowersAsync(resOwnerId);
        }
        public async Task<int?> GetResOwnerIdByResIdAsync(int resId)
        {
            return await _friendDAO.GetResOwnerIdByResIdAsync(resId);
        }

    }
}