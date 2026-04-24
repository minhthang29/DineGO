using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CustomerVoucher
    {
        [Key]
        public int customer_voucher_id { get; set; }

        public int cus_id { get; set; }
        public int voucher_id { get; set; }

        [Required]
        public int customer_voucher_quantity { get; set; }

        public Customer? customer { get; set; }
        public Voucher? voucher { get; set; }
    }
}