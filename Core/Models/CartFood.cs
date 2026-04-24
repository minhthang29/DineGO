using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CartFood
    {
        [Key]
        public int cart_food_id { get; set; }

        public int? food_id { get; set; }
        public int? cart_id { get; set; }
        public bool? is_buy { get; set; }
        public int? food_quantity { get; set; }

        public Cart? cart { get; set; }
        public Food? food { get; set; }
    }
}