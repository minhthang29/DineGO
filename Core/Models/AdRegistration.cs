using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AdRegistration
    {
        [Key]
        public int ad_id { get; set; }

        // FK Slot
        public int slot_id { get; set; }
        public AdSlot slot { get; set; }

        // FK RestaurantOwner (người thuê quảng cáo)
        public int res_owner_id { get; set; }
        public RestaurantOwner restaurantOwner { get; set; }

        [Required, MaxLength(300)]
        public string ad_image_url { get; set; }

        [MaxLength(300)]
        public string? ad_link_url { get; set; }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public bool is_active { get; set; } // Còn hiệu lực hay hết hạn
    }
}