using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class AdSlotDto
    {
        public int slot_id { get; set; }
        public string slot_name { get; set; }
        public int slot_type { get; set; }
        public decimal slot_price { get; set; }
        public bool slot_is_active { get; set; }
        public bool occupied { get; set; }

    }
}