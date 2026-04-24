using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Models.AdminModel
{
    public class RestaurantViewDetailModel
    {
        public Restaurant Restaurant { get; set; }
        public List<Menu> Menus { get; set; }
        public Menu Menu { get; set; }
        public List<Order> listOrder { get; set; }
        public List<Reservation> listReservation { get; set; }
    }
}