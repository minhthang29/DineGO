using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{


    public class RestaurantDetailViewModel
    {
        public Restaurant Restaurant { get; set; }
        public List<MenuWithFoodsViewModel> MenusWithFoods { get; set; }

        public List<RestaurantRating> Ratings { get; set; }

        public bool HasCompletedOrder { get; set; }

        public int? CurrentCustomerId { get; set; }
        public RestaurantRating? MyRating { get; set; }
        public List<Customer> Customers { get; set; } = new();



    }

}