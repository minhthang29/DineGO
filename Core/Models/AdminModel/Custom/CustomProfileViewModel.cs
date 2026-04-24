using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.AdminModel.Custom
{
    public class CustomProfileViewModel
    {
        public Customer Customer { get; set; }
        public List<RestaurantOwner> RestaurantOwners { get; set; }
        public List<Restaurant> Restaurant { get; set; }
        public List<Reservation> Reservation { get; set; }
        public List<Reservation> ConfirmedOrRejectedReservations { get; set; }
        public List<Reservation> PendingReservations { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}
