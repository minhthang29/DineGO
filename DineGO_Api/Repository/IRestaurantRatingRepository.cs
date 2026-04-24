using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IRestaurantRatingRepository
    {
        List<RestaurantRating> GetRatingsByRestaurantId(int resId);
        RestaurantRating? GetRatingByCustomer(int cusId, int resId);
        void AddRating(RestaurantRating rating);
        void UpdateRating(RestaurantRating rating);
        void UpdateAverageRating(int resId);
        bool HasCompletedOrder(int cusId, int resId);


    }
}