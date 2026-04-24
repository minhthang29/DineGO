using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Client.Custom
{
    public class CheckOutViewModel
    {
        public Customer Customer { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<CheckoutFoodItem> SelectedFoods { get; set; } // Thêm dòng này
    }
} 