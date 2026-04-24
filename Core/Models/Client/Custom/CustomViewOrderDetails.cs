using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class CustomViewOrderDetails
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<CartFood> listCartFood { get; set; }
        public List<Food> listFood { get; set; }
        public Voucher Voucher { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}