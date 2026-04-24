using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Voucher
    {
        [Key]
        public int voucher_id { get; set; }

        public int? ad_id { get; set; }

        [Required, MaxLength(50)]
        public string voucher_code { get; set; }

        [Required]
        public decimal voucher_discount { get; set; }

        [Required]
        public DateTime voucher_start_date { get; set; }

        [Required]
        public DateTime voucher_end_date { get; set; }

        public int? voucher_stock { get; set; }
        public bool voucher_is_active { get; set; }
        public bool voucher_is_deleted { get; set; }
        public int voucher_type { get; set; } // 0: Percentage, 1: Fixed Amount
        public int voucher_apply_type { get; set; } // 0: All, 1: Specific customer, 2: Other
        public int? required_points { get; set; }
        public decimal? voucher_cap_amount { get; set; } // Chỉ áp dụng khi loại % (voucher_type = 0)
        public Admin? admin { get; set; }

        public ICollection<CustomerVoucher>? customerVouchers { get; set; }
    }

}