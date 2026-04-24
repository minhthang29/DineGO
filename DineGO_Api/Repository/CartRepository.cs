using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using DineGO_Api.Data;
using DineGO_Api.Models.CartItemModel;

namespace DineGO_Api.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDAO _cartDao;
        private readonly CartFoodDAO _cartFoodDao;

        public CartRepository(CartDAO cartDao, CartFoodDAO cartFoodDao)
        {
            _cartDao = cartDao;
            _cartFoodDao = cartFoodDao;
        }

        public List<CartItemViewModel> GetGroupedCartByCustomer(int cusId)
        {
            var cartFoods = _cartFoodDao.GetCartFoods()
                .Where(cf => cf.cart != null && cf.cart.cus_id == cusId && cf.is_buy == false)
                .ToList();

            var grouped = cartFoods
                .GroupBy(cf => cf.cart!.restaurant!.res_name)
                .Select(g => new CartItemViewModel
                {
                    RestaurantName = g.Key,
                    RestaurantId = g.Select(cf => cf.cart!.restaurant!.res_id).FirstOrDefault(),
                    restaurant = g.Select(cf => cf.cart!.restaurant!).FirstOrDefault(),
                    Items = g.Select(cf => new CartFoodItem
                    {
                        CartFoodId = cf.cart_food_id,
                        FoodName = cf.food?.food_name ?? "",
                        Price = cf.food?.food_price ?? 0,
                        Quantity = cf.food_quantity ?? 1,
                        IsChecked = cf.is_buy ?? false,  // tận dụng luôn is_buy hiện tại
                    }).ToList()
                })
                .ToList();

            return grouped;
        }

        public void DeleteCartItem(int cartFoodId)
        {
            var cartFood = _cartFoodDao.FindCartFoodById(cartFoodId);
            if (cartFood == null) return;

            int? cartId = cartFood.cart_id;

            _cartFoodDao.DeleteCartFood(cartFoodId);

            if (cartId.HasValue)
            {
                var remainingItems = _cartFoodDao.GetCartFoods()
                    .Where(cf => cf.cart_id == cartId && cf.is_buy == false)
                    .ToList();

                if (!remainingItems.Any())
                {
                    _cartDao.DeleteCart(cartId.Value);
                }
            }
        }


        public bool UpdateQuantity(int cartFoodId, int quantity)
        {
            var item = _cartFoodDao.FindCartFoodById(cartFoodId);
            if (item == null) return false;
            item.food_quantity = quantity;
            _cartFoodDao.UpdateCartFood(item);
            return true;
        }
        public void AddFoodToCart(int cusId, int foodId, int quantity)
        {
            _cartDao.AddFoodToCart(cusId, foodId, quantity);
        }

        public bool UpdateIsBuy(List<int> cartFoodIds)
        {
            var items = _cartFoodDao.GetCartFoods()
                .Where(cf => cartFoodIds.Contains(cf.cart_food_id))
                .ToList();

            if (!items.Any()) return false;

            foreach (var cf in items)
            {
                cf.is_buy = true;
                _cartFoodDao.UpdateCartFood(cf);
            }

            return true;
        }

        public List<CartFood> GetCartFoods()
        {
            return _cartFoodDao.GetCartFoods();
        }

        public Core.Models.Client.Custom.CheckOutViewModel GetCheckOutInfo(int customerId, string selectedIds)
        {
            // Lấy thông tin khách hàng
            var customer = _cartDao.GetCustomerById(customerId);

            if (customer == null)
                throw new Exception("Customer not found");
            var customerInfo = customer;

            // Parse selectedIds thành List<int>
            List<int> selectedCartFoodIds = new List<int>();
            if (!string.IsNullOrEmpty(selectedIds))
            {
                selectedCartFoodIds = selectedIds.Split(',').Select(id => int.Parse(id)).ToList();
            }

            // Lấy danh sách món ăn theo id
            var foods = _cartFoodDao.GetCartFoods()
                .Where(cf => selectedCartFoodIds.Contains(cf.cart_food_id))
                .ToList();

            var foodViewModels = foods.Select(cf => new Core.Models.Client.Custom.CheckoutFoodItem
            {
                CartFoodId = cf.cart_food_id,
                FoodName = cf.food?.food_name ?? "",
                FoodId = cf.food?.food_id ?? 0,
                FoodImage = cf.food?.food_image ?? "",
                Price = cf.food?.food_price ?? 0,
                Quantity = cf.food_quantity ?? 1,
                IsChecked = cf.is_buy ?? false,
                Discount = 0,
                // Lấy tên nhà hàng
                RestaurantName = cf.cart?.restaurant?.res_name ?? "",
                RestaurantId = cf.cart?.restaurant?.res_id ?? 0,
                PrepTime = cf.food?.food_prep_time
            }).ToList();
            var restaurant = foods.FirstOrDefault()?.cart?.restaurant;
            var checkoutInfo = new Core.Models.Client.Custom.CheckOutViewModel
            {
                Customer = customerInfo,
                Restaurant = restaurant,
                SelectedFoods = foodViewModels // Thêm property này vào ViewModel nếu chưa có
            };

            return checkoutInfo;
        }
        /// <summary>
        /// Xóa các cart items đã được chọn khỏi giỏ hàng
        /// </summary>
        public bool ClearSelectedCartItems(List<int> cartFoodIds)
        {
            try
            {
                foreach (var cartFoodId in cartFoodIds)
                {
                    var cartFood = _cartFoodDao.FindCartFoodById(cartFoodId);
                    if (cartFood != null)
                    {
                        _cartFoodDao.DeleteCartFood(cartFoodId);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}