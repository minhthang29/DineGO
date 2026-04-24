using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.EntityFrameworkCore;
namespace DineGO_Api.Data
{
    public class CartDAO
    {
        private readonly ApplicationDbContext _context;
        public CartDAO(ApplicationDbContext context) => _context = context;

        public void AddFoodToCart(int cusId, int foodId, int quantity)
        {
            var food = _context.Foods
                .Include(f => f.menu)
                .ThenInclude(m => m.restaurant)
                .FirstOrDefault(f => f.food_id == foodId);

            if (food == null)
                throw new Exception("Food not found");

            if (food.menu == null || food.menu.res_id == 0)
                throw new Exception("Invalid menu/restaurant");

            int resId = food.menu.res_id;

            // XÓA TẤT CẢ CART KHÁC NHÀ HÀNG HIỆN TẠI
            var otherCarts = _context.Carts
                .Where(c => c.cus_id == cusId && c.res_id != resId)
                .ToList();
            foreach (var carts in otherCarts)
            {
                var cartFoods = _context.CartFoods.Where(cf => cf.cart_id == carts.cart_id).ToList();
                _context.CartFoods.RemoveRange(cartFoods);
                _context.Carts.Remove(carts);
            }
            _context.SaveChanges();

            // TIẾP TỤC XỬ LÝ NHƯ CŨ
            var cart = _context.Carts.FirstOrDefault(c => c.cus_id == cusId && c.res_id == resId);
            if (cart == null)
            {
                cart = new Cart
                {
                    cus_id = cusId,
                    res_id = resId
                };
                _context.Carts.Add(cart);
                _context.SaveChanges();
            }

            var cartFood = _context.CartFoods.FirstOrDefault(cf =>
                cf.cart_id == cart.cart_id &&
                cf.food_id == foodId &&
                cf.is_buy == false);

            if (cartFood != null)
            {
                cartFood.food_quantity += quantity;
            }
            else
            {
                cartFood = new CartFood
                {
                    cart_id = cart.cart_id,
                    food_id = foodId,
                    food_quantity = quantity,
                    is_buy = false
                };
                _context.CartFoods.Add(cartFood);
            }

            _context.SaveChanges();
        }




        // Get all Carts
        public List<Cart> GetCategories()
        {
            try
            {
                return _context.Carts.ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching Carts: {e.Message}");
            }
        }

        // Get Cart by ID
        public Cart FindCartById(int id)
        {
            try
            {
                return _context.Carts.SingleOrDefault(x => x.cart_id == id);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Cart: {e.Message}");
            }
        }

        // Save a new Cart
        public void SaveCart(Cart Cart)
        {
            try
            {
                _context.Carts.Add(Cart);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving Cart: {e.Message}");
            }
        }

        // Update Cart details
        public void UpdateCart(Cart Cart)
        {
            try
            {
                _context.Entry(Cart).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating Cart: {e.Message}");
            }
        }

        // Delete Cart by ID
        public void DeleteCart(int id)
        {
            try
            {
                var Cart = _context.Carts.SingleOrDefault(x => x.cart_id == id);
                if (Cart != null)
                {
                    _context.Carts.Remove(Cart);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting Cart: {e.Message}");
            }
        }

        // Get Customer by ID
        public Customer GetCustomerById(int customerId)
        {
            try
            {
                return _context.Customers.FirstOrDefault(c => c.cus_id == customerId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Customer: {e.Message}");
            }
        }

        // Get Latest Order by Customer ID
        public Order GetLatestOrderByCustomer(int customerId)
        {
            try
            {
                return _context.Orders
                    .Include(o => o.restaurant)
                    .Where(o => o.cus_id == customerId)
                    .OrderByDescending(o => o.order_date)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding latest Order: {e.Message}");
            }
        }

        // Get Order Details by Order ID
        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            try
            {
                return _context.OrderDetails
                    .Where(od => od.order_id == orderId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding Order Details: {e.Message}");
            }
        }
    }
}