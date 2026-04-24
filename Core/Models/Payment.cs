using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Payment
    {
        [Key]
        public int pay_id { get; set; }

        public int? cart_id { get; set; }
        public int cus_id { get; set; }
        public int? reser_id { get; set; }

        [Required]
        public decimal pay_price { get; set; }

        public int? pay_status { get; set; }
        public bool pay_is_deleted { get; set; }

        [Required]
        public DateTime pay_created_date { get; set; }

        public decimal? pay_price_discount { get; set; }

        public Cart? cart { get; set; }
        public Customer? customer { get; set; }
        public Reservation? reservation { get; set; }
    }
}