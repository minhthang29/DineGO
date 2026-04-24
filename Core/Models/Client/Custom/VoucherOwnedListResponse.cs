using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class VoucherOwnedListResponse
    {
        public int CustomerBalance { get; set; }
        public List<VoucherOwnedViewmodel> Vouchers { get; set; }
    }
}