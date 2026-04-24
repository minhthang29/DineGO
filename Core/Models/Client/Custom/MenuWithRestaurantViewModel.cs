using System;
using System.Collections.Generic;
using Core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class MenuWithRestaurantViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<Menu> Menus { get; set; }
        public Menu Menu { get; set; }  
         public Dictionary<int, int> MenuFoodCounts { get; set; } = new();
    }
}