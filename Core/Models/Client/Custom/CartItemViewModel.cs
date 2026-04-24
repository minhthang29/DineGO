using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
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

        public bool IsChecked { get; set; }  // ✅ Check món hay không
        public decimal Discount { get; set; } = 0;  // ✅ Gắn giảm giá nếu có

        public decimal Total => (Price * Quantity) - Discount;
    }


    public class UpdateQuantityRequest
    {
        public int CartFoodId { get; set; }
        public int Quantity { get; set; }
    }
}