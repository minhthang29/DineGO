using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class RestaurantDAO
    {
        private readonly ApplicationDbContext _context;

        public RestaurantDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all Restaurants
        public List<Restaurant> GetRestaurants()
        {
            try
            {
                return _context.Restaurants.Where(x => x.res_is_deleted == false && x.res_is_authorized == true && x.res_is_use == true).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Restaurants: {e.Message}");
            }
        }
        public List<Restaurant> GetRestaurantsForAdmin()
        {
            try
            {
                return _context.Restaurants.Where(x => x.res_is_deleted == false).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Restaurants: {e.Message}");
            }
        }

        public Restaurant FindRestaurantById(int id)
        {
            try
            {
                return _context.Restaurants.SingleOrDefault(x => x.res_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding restaurant: {e.Message}");
            }
        }

        // Get restaurant by ID
        public List<Restaurant> FindRestaurantByRestaurantOwnerId(int ownerId)
        {
            try
            {

                return _context.Restaurants.Where(x => x.res_owner_id == ownerId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding restaurant: {e.Message}");
            }
        }

        // Save a new restaurant
        public void SaveRestaurant(Restaurant restaurant)
        {   
            try
            {
                _context.Restaurants.Add(restaurant);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving restaurant: {e.Message}");
            }
        }

        // Update restaurant details
        public void UpdateRestaurant(Restaurant restaurant)
        {
            try
            {
                _context.Entry(restaurant).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating restaurant: {e.Message}");
            }
        }

        // Delete restaurant by ID
        public void DeleteRestaurant(int id)
        {
            try
            {
                var restaurant = _context.Restaurants.SingleOrDefault(x => x.res_id == id);
                if (restaurant != null)
                {
                    restaurant.res_is_deleted = true; // Đánh dấu là không hoạt động
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting restaurant: {e.Message}");
            }
        }
        // Delete restaurant by ID
        public void BlockRestaurant(int id)
        {
            try
            {
                var restaurant = _context.Restaurants.SingleOrDefault(x => x.res_id == id);
                if (restaurant != null)
                {
                    restaurant.res_is_use = false; // Soft delete: mark as inactive
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error blocking restaurant: {e.Message}");
            }
        }
        // Delete restaurant by ID
        public void ActiveRestaurant(int id)
        {
            try
            {
                var restaurant = _context.Restaurants.SingleOrDefault(x => x.res_id == id);
                if (restaurant != null)
                {
                    restaurant.res_is_use = true; // Soft delete: mark as inactive
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error activating restaurant: {e.Message}");
            }
        }
    }
}
