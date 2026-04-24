using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using Core.Models;

namespace DineGO_Api.Repository
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly RestaurantDAO _restaurantDAO;
        private readonly FollowerDAO _followerDAO;
        public RestaurantRepository(RestaurantDAO restaurantDAO, FollowerDAO followerDAO)
        {
            _restaurantDAO = restaurantDAO;
            _followerDAO = followerDAO;
        }
        public List<Restaurant> GetRestaurants() => _restaurantDAO.GetRestaurants();
        public List<Restaurant> GetRestaurantsForAdmin() => _restaurantDAO.GetRestaurantsForAdmin();
        public Restaurant FindRestaurantById(int Id) => _restaurantDAO.FindRestaurantById(Id);
        public List<Restaurant> FindRestaurantByRestaurantOwnerId(int ownerId) => _restaurantDAO.FindRestaurantByRestaurantOwnerId(ownerId);
        public void SaveRestaurant(Restaurant p) => _restaurantDAO.SaveRestaurant(p);
        public void UpdateRestaurant(Restaurant p) => _restaurantDAO.UpdateRestaurant(p);
        public void DeleteRestaurant(int Id) => _restaurantDAO.DeleteRestaurant(Id);
        public void BlockRestaurant(int Id) => _restaurantDAO.BlockRestaurant(Id);
        public void ActiveRestaurant(int Id) => _restaurantDAO.ActiveRestaurant(Id);

        public List<Restaurant> SearchRestaurants(string name, string address)
        {
            var restaurants = _restaurantDAO.GetRestaurants();

            if (!string.IsNullOrEmpty(name))
            {
                restaurants = restaurants.Where(r => r.res_name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(address))
            {
                restaurants = restaurants.Where(r => r.res_address.Contains(address, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return restaurants;
        }
        public bool FollowRestaurant(int cus_id, int res_id)
        {
            return _followerDAO.AddFollower(cus_id, res_id);
        }

        public bool UnfollowRestaurant(int cus_id, int res_id)
        {
            return _followerDAO.RemoveFollower(cus_id, res_id);
        }

        public bool IsFollowingRestaurant(int cus_id, int res_id)
        {
            return _followerDAO.IsFollowing(cus_id, res_id);
        }
    }
}