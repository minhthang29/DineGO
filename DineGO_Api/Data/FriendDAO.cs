using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class FriendDAO
    {
        private readonly ApplicationDbContext _context;

        public FriendDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AreFriendsAsync(int senderId, int receiverId)
        {
            // Chỉ true nếu người được gửi (receiver) đã chấp nhận → nghĩa là có dòng receiver → sender
            return await _context.Friends.AnyAsync(f =>
                f.customer_id == receiverId && f.friend_customer_id == senderId);
        }

        public async Task<List<Customer>> GetFriendsAsync(int userId)
        {
            var friendIds = await _context.Friends
                .Where(f => f.customer_id == userId || f.friend_customer_id == userId)
                .Select(f => f.customer_id == userId ? f.friend_customer_id : f.customer_id)
                .Distinct()
                .ToListAsync();

            return await _context.Customers
                .Where(c => friendIds.Contains(c.cus_id))
                .ToListAsync();
        }

        public async Task<List<Customer>> SearchUsersAsync(string keyword, int currentUserId)
        {
            keyword = keyword.ToLower();

            // Lấy danh sách bạn đã kết bạn (1 chiều hoặc 2 chiều)
            var myFriendIds = await _context.Friends
                .Where(f => f.customer_id == currentUserId || f.friend_customer_id == currentUserId)
                .Select(f => f.customer_id == currentUserId ? f.friend_customer_id : f.customer_id)
                .Distinct()
                .ToListAsync();

            // Thêm chính mình vào luôn để không search ra bản thân
            myFriendIds.Add(currentUserId);

            return await _context.Customers
                .Where(c =>
                    ((c.cus_name ?? "").ToLower().Contains(keyword) || (c.cus_username ?? "").ToLower().Contains(keyword))
                    && !myFriendIds.Contains(c.cus_id)
                )
                .ToListAsync();
        }

        public async Task AddFriendAsync(Friend friend)
        {
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
        }

        public async Task AddFriendWithResOwnerAsync(int customerId, int resOwnerId)
        {
            var f1 = new Friend
            {
                customer_id = customerId,
                friend_customer_id = resOwnerId,
                is_resowner = true,
                created_at = DateTime.UtcNow
            };

            var f2 = new Friend
            {
                customer_id = resOwnerId,
                friend_customer_id = customerId,
                is_resowner = false,
                created_at = DateTime.UtcNow
            };

            _context.Friends.AddRange(f1, f2);
            await _context.SaveChangesAsync();
        }
        public async Task<List<RestaurantOwner>> GetFriendsByFollowerAsync(int cusId)
        {
            return await _context.Followers
                .Where(f => f.cus_id == cusId)
                .Select(f => f.restaurantOwner)
                .Where(ro => ro != null && !ro.res_owner_is_deleted)
                .ToListAsync();
        }
        public async Task<List<Customer>> GetFriendsByResOwnerAsync(int resOwnerId)
        {
            var friendIds = await _context.Friends
                .Where(f => f.customer_id == resOwnerId && !f.is_resowner)
                .Select(f => f.friend_customer_id)
                .ToListAsync();

            return await _context.Customers
                .Where(c => friendIds.Contains(c.cus_id))
                .ToListAsync();
        }
        public async Task<List<Customer>> GetCustomerFollowersAsync(int resOwnerId)
        {
            return await _context.Followers
                .Where(f => f.res_owner_id == resOwnerId)
                .Select(f => f.customer)
                .Where(c => c != null && c.cus_is_use)
                .ToListAsync();
        }

        // 1. Customer → List<Restaurant> mà họ follow thông qua ResOwner
        public async Task<List<Restaurant>> GetRestaurantsByCustomerFollowerAsync(int cusId)
        {
            var resOwnerIds = await _context.Followers
                .Where(f => f.cus_id == cusId)
                .Select(f => f.res_owner_id)
                .ToListAsync();

            return await _context.Restaurants
                .Where(r => resOwnerIds.Contains(r.res_owner_id) && r.res_is_use && !r.res_is_deleted)
                .ToListAsync();
        }

        // 2. ResOwner → List<Restaurant kèm List<Customer follower>
        public async Task<Dictionary<Restaurant, List<Customer>>> GetRestaurantsAndFollowersAsync(int resOwnerId)
        {
            var restaurants = await _context.Restaurants
                .Where(r => r.res_owner_id == resOwnerId && r.res_is_use && !r.res_is_deleted)
                .ToListAsync();

            var resWithFollowers = new Dictionary<Restaurant, List<Customer>>();

            foreach (var res in restaurants)
            {
                var customers = await _context.Followers
                    .Where(f => f.res_owner_id == resOwnerId)
                    .Select(f => f.customer)
                    .Where(c => c != null && c.cus_is_use)
                    .ToListAsync();

                resWithFollowers[res] = customers;
            }

            return resWithFollowers;
        }
        public async Task<int?> GetResOwnerIdByResIdAsync(int resId)
        {
            var res = await _context.Restaurants.FindAsync(resId);
            return res?.res_owner_id;
        }

    }
}