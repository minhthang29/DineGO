using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Data
{
    public class FollowerDAO
    {
        private readonly ApplicationDbContext _context;

        public FollowerDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddFollower(int cus_id, int res_id)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.res_id == res_id);
            if (restaurant == null) return false;

            int res_owner_id = restaurant.res_owner_id;

            bool exists = _context.Followers.Any(f => f.cus_id == cus_id && f.res_owner_id == res_owner_id);
            if (exists) return false;

            var follower = new Follower
            {
                cus_id = cus_id,
                res_owner_id = res_owner_id,
                follower_created = DateTime.Now
            };

            _context.Followers.Add(follower);

            var owner = _context.RestaurantOwners.FirstOrDefault(o => o.res_owner_id == res_owner_id);
            if (owner != null)
            {
                owner.res_owner_follower_count++;
            }

            _context.SaveChanges();
            return true;
        }

        public bool RemoveFollower(int cus_id, int res_id)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.res_id == res_id);
            if (restaurant == null) return false;

            int res_owner_id = restaurant.res_owner_id;

            var existing = _context.Followers.FirstOrDefault(f => f.cus_id == cus_id && f.res_owner_id == res_owner_id);
            if (existing == null) return false;

            _context.Followers.Remove(existing);

            var owner = _context.RestaurantOwners.FirstOrDefault(o => o.res_owner_id == res_owner_id);
            if (owner != null && owner.res_owner_follower_count > 0)
            {
                owner.res_owner_follower_count--;
            }

            _context.SaveChanges();
            return true;
        }

        public bool IsFollowing(int cus_id, int res_id)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.res_id == res_id);
            if (restaurant == null) return false;

            int res_owner_id = restaurant.res_owner_id;
            return _context.Followers.Any(f => f.cus_id == cus_id && f.res_owner_id == res_owner_id);
        }
    }
}