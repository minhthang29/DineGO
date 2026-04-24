using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class GiftVoucherRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int VoucherId
        {
            get; set;
        }
    }
}