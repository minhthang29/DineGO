using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class FoodWithMenuViewModel
    {
        public List<Food> Foods { get; set; }
        public Menu Menu { get; set; }
        public Restaurant Restaurant { get; set; } 
    }

}