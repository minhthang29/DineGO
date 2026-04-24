using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace DineGO_Api.Data
{
    public class CartFoodDAO
    {
        private readonly ApplicationDbContext _context;

        public CartFoodDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all CartFoods
        public List<CartFood> GetCartFoods()
        {
            try
            {
                return _context.CartFoods
                    .Include(cf => cf.cart)
                        .ThenInclude(c => c.restaurant)
                    .Include(cf => cf.food)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching CartFoods: {e.Message}");
            }
        }

        // Get CartFood by ID
        public CartFood FindCartFoodById(int id)
        {
            try
            {
                return _context.CartFoods.SingleOrDefault(x => x.cart_food_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding CartFood: {e.Message}");
            }
        }

        // Save a new CartFood
        public void SaveCartFood(CartFood CartFood)
        {
            try
            {
                _context.CartFoods.Add(CartFood);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving CartFood: {e.Message}");
            }
        }

        // Update CartFood details
        public void UpdateCartFood(CartFood CartFood)
        {
            try
            {
                _context.Entry(CartFood).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating CartFood: {e.Message}");
            }
        }

        // Delete CartFood by ID
        public void DeleteCartFood(int id)
        {
            try
            {
                var CartFood = _context.CartFoods.SingleOrDefault(x => x.cart_food_id == id);
                if (CartFood != null)
                {
                    _context.CartFoods.Remove(CartFood);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting CartFood: {e.Message}");
            }
        }
    }
}