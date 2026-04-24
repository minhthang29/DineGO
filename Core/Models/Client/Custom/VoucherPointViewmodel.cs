using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class VoucherPointViewmodel
    {
        public class VoucherListResponse
        {
            public int CustomerBalance { get; set; }
            public List<VoucherItem> Vouchers { get; set; }
        }

        public class VoucherItem
        {
            public int voucher_id { get; set; }
            public string voucher_code { get; set; }
            public decimal voucher_discount { get; set; }
            public DateTime voucher_start_date { get; set; }
            public DateTime voucher_end_date { get; set; }
            public int? voucher_stock { get; set; }
            public int voucher_type { get; set; }
            public int voucher_apply_type { get; set; }
            public int? required_points { get; set; }
        }

    }
}