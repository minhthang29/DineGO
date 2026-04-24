using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class CheckoutFoodItem
    {
        public int CartFoodId { get; set; }
        public string FoodName { get; set; }
        public int FoodId { get; set; }
        public string FoodImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsChecked { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal Total => (Price * Quantity) - Discount;
        public string RestaurantName { get; set; }
        public int RestaurantId { get; set; }
        public int? PrepTime { get; set; } // Thời gian chuẩn bị món ăn
    }
}