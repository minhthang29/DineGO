using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DineGO_Api.Data
{
    public class RestaurantOwnerDAO
    {
        private readonly ApplicationDbContext _context;

        public RestaurantOwnerDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all RestaurantOwners
        public List<RestaurantOwner> GetRestaurantOwners()
        {
            try
            {
                return _context.RestaurantOwners.Where(x => x.res_owner_is_deleted == false).ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Restaurant Owners: {e.Message}");
            }
        }

        // Get RestaurantOwner by ID
        public RestaurantOwner FindRestaurantOwnerById(int id)
        {
            try
            {
                return _context.RestaurantOwners.SingleOrDefault(x => x.res_owner_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Blog: {e.Message}");
            }
        }

        // Get RestaurantOwner by customer ID
        public List<RestaurantOwner> FindRestaurantOwnersByCusId(int cusId)
        {
            try
            {
                return _context.RestaurantOwners
                       .Where(x => x.cus_id == cusId && x.res_owner_is_deleted == false)
                       .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Restaurant Owners: {e.Message}");
            }
        }

        // Save a new RestaurantOwner
        public void SaveRestaurantOwner(RestaurantOwner restaurantOwner)
        {
            try
            {
                _context.RestaurantOwners.Add(restaurantOwner);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Restaurant Owner: {e.Message}");
            }
        }

        // Update RestaurantOwner details
        public void UpdateRestaurantOwner(RestaurantOwner restaurantOwner)
        {
            try
            {
                _context.Entry(restaurantOwner).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Restaurant Owner: {e.Message}");
            }
        }

        // Delete RestaurantOwner by ID
        public void DeleteRestaurantOwner(int id)
        {
            try
            {
                var restaurantOwner = _context.RestaurantOwners.SingleOrDefault(x => x.res_owner_id == id);
                if (restaurantOwner != null)
                {
                    restaurantOwner.res_owner_is_deleted = true; // Xóa mềm
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Restaurant Owner: {e.Message}");
            }
        }
        
    }
}
