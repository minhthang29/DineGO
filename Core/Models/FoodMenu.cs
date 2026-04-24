using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class FoodMenu
    {
        [Key]
        public int food_menu_id { get; set; }

        public int? food_id { get; set; }
        public int? menu_id { get; set; }

        public Food? food { get; set; }
        public Menu? menu { get; set; }
    }
}