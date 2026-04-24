using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class RestaurantRating
    {
        [Key]
        public int rating_id { get; set; }

        [Required]
        public int cus_id { get; set; }

        [Required]
        public int res_id { get; set; }

        [Range(1, 5)]
        public int rating_value { get; set; }

        [MaxLength(500)]
        public string? rating_comment { get; set; }

        public DateTime rating_date { get; set; } = DateTime.Now;

        public Customer? customer { get; set; }
        public Restaurant? restaurant { get; set; }
        
    }
}
