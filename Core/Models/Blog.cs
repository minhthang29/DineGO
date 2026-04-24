using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Blog
    {
        [Key]
        public int blog_id { get; set; }

        public int? res_owner_id { get; set; }
        // Thêm trường này
        public int? ad_id { get; set; }
        public Admin? admin { get; set; }

        [Required, MaxLength(150)]
        public string blog_title { get; set; }

        [Required]
        public string blog_information { get; set; }

        [Required]
        public DateTime blog_date { get; set; }

        public string? blog_image { get; set; }
        public bool blog_is_deleted { get; set; }

        public RestaurantOwner? restaurantOwner { get; set; }
    }
}