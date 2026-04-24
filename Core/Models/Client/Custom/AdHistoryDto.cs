using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class AdHistoryDto
    {
        public int history_id { get; set; }
        public int ad_id { get; set; }
        public int slot_id { get; set; }
        public int res_owner_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTime archived_date { get; set; }
    }
}