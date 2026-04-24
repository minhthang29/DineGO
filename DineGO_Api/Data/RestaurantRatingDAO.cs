using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Data
{
    public class RestaurantRatingDAO
    {
        private readonly ApplicationDbContext _context;

        public RestaurantRatingDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all ratings of a restaurant
        /// </summary>
        public List<RestaurantRating> GetRatingsByRestaurantId(int resId)
        {
            return _context.RestaurantRatings
                .Where(r => r.res_id == resId)
                .OrderByDescending(r => r.rating_date)
                .ToList();
        }

        /// <summary>
        /// Get a rating by customer for a specific restaurant
        /// </summary>
        public RestaurantRating? GetRatingByCustomer(int cusId, int resId)
        {
            return _context.RestaurantRatings
                .FirstOrDefault(r => r.cus_id == cusId && r.res_id == resId);
        }

        /// <summary>
        /// Add new rating
        /// </summary>
        public void AddRating(RestaurantRating rating)
        {
            _context.RestaurantRatings.Add(rating);
            _context.SaveChanges();
            UpdateAverageRating(rating.res_id);
        }

        /// <summary>
        /// Update existing rating
        /// </summary>
        public void UpdateRating(RestaurantRating rating)
        {
            _context.RestaurantRatings.Update(rating);
            _context.SaveChanges();
            UpdateAverageRating(rating.res_id);
        }

        public void UpdateAverageRating(int resId)
        {
            var ratings = _context.RestaurantRatings
                .Where(r => r.res_id == resId)
                .ToList();

            if (ratings.Count == 0) return;

            double average = ratings.Average(r => r.rating_value);
            int count = ratings.Count;

            var restaurant = _context.Restaurants.Find(resId);
            if (restaurant != null)
            {
                restaurant.res_rate = (decimal)Math.Round(average, 2);
                restaurant.res_rate_count = count;
                _context.SaveChanges();
            }
        }
        public bool HasCompletedOrder(int cusId, int resId)
        {
            return _context.Orders.Any(o =>
                o.cus_id == cusId &&
                o.res_id == resId &&
                o.order_status == 3 
            );
        }

    }
}
