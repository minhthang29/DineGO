using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client.Custom
{
    public class AdRegistrationResponseDto
    {
        public int ad_id { get; set; }
        public int slot_id { get; set; }
        public int res_owner_id { get; set; }
        public string ad_image_url { get; set; }
        public string? ad_link_url { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_active { get; set; }
        public string slot_name { get; set; }
        public int slot_type { get; set; }
    }
}