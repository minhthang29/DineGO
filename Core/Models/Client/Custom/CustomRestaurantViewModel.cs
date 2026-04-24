using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.Client.Custom
{
    /// <summary>
    /// Represents the view model for booking a reservation.
    /// </summary>
    /// <author>Thangtm</author>
    public class CustomRestaurantViewModel
    {
        public Restaurant Restaurant { get; set; }
        public RestaurantOwner RestaurantOwner { get; set; }
        public Customer Customer { get; set; }
        public List<Restaurant> Restaurants { get; set; }
    }
}