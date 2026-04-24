using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Client.Custom
{
    public class DeliveryTrackingViewModel
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime OrderDate { get; set; }
        public int DeliveryStatus { get; set; }
        public string StatusText { get; set; }
        public string StatusClass { get; set; }
        public decimal OrderTotal { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int OrderDetailId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public string FoodImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 