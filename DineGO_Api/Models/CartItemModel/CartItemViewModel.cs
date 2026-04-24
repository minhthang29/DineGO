using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace DineGO_Api.Models.CartItemModel
{
    public class CartItemViewModel
    {
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }  // ✅ Thêm RestaurantId để biết món này thuộc nhà hàng nào
        public List<CartFoodItem> Items { get; set; }
        public Restaurant restaurant { get; set; }
    }

    public class CartFoodItem
    {
        public int CartFoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public bool IsChecked { get; set; }
        public decimal Discount { get; set; } = 0;

        public decimal Total => (Price * Quantity) - Discount;
    }


    public class UpdateQuantityRequest
    {
        public int CartFoodId { get; set; }
        public int Quantity { get; set; }
    }

    public class CheckUpdateRequest
    {
        public int CartFoodId { get; set; }
        public bool IsChecked { get; set; }
    }

}