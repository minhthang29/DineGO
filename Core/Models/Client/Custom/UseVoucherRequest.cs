using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class UseVoucherRequest
    {
        public int CustomerId { get; set; }
        public string VoucherCode { get; set; }
    }
}