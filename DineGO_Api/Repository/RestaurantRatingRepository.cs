using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;

namespace DineGO_Api.Repository
{
    public class RestaurantRatingRepository : IRestaurantRatingRepository
    {
        private readonly RestaurantRatingDAO _dao;

        public RestaurantRatingRepository(RestaurantRatingDAO dao)
        {
            _dao = dao;
        }

        public List<RestaurantRating> GetRatingsByRestaurantId(int resId) => _dao.GetRatingsByRestaurantId(resId);

        public RestaurantRating? GetRatingByCustomer(int cusId, int resId) => _dao.GetRatingByCustomer(cusId, resId);

        public void AddRating(RestaurantRating rating) => _dao.AddRating(rating);

        public void UpdateRating(RestaurantRating rating) => _dao.UpdateRating(rating);

        public void UpdateAverageRating(int resId)
        {
            _dao.UpdateAverageRating(resId);
        }
        public bool HasCompletedOrder(int cusId, int resId)
        {
            return _dao.HasCompletedOrder(cusId, resId);
        }

    }
}
