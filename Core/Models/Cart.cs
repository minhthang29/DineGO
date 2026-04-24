using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Cart
    {
        [Key]
        public int cart_id { get; set; }

        public int? res_id { get; set; }
        public int? cus_id { get; set; }
        public bool cart_is_deleted { get; set; }
        public Customer? customer { get; set; }
        public Restaurant? restaurant { get; set; }

        public ICollection<CartFood>? cartFoods { get; set; }
        public ICollection<Payment>? payments { get; set; }
    }

}