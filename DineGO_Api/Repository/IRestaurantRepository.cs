using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Repository
{
    public interface IRestaurantRepository
    {
        List<Restaurant> GetRestaurants();
        List<Restaurant> GetRestaurantsForAdmin();
        Restaurant FindRestaurantById(int ID);
        List<Restaurant> FindRestaurantByRestaurantOwnerId(int ownerId);

        void SaveRestaurant(Restaurant p);

        void UpdateRestaurant(Restaurant p);

        void DeleteRestaurant(int p);
        void BlockRestaurant(int p);
        void ActiveRestaurant(int p);

        List<Restaurant> SearchRestaurants(string name, string address);
        bool FollowRestaurant(int cus_id, int res_id);
        bool UnfollowRestaurant(int cus_id, int res_id);
        bool IsFollowingRestaurant(int cus_id, int res_id);

    }
}