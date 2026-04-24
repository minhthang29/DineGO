using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Core.Models
{
    public class CustomerPoint
    {
        [Key]
        public int point_id { get; set; }

        [Required]
        public int cus_id { get; set; }

        [Required]
        public int point_balance { get; set; }

        public DateTime created_date { get; set; }
        public DateTime last_updated { get; set; }

        // Quan hệ
        public Customer? customer { get; set; }
        public ICollection<CustomerPointHistory>? pointHistories { get; set; }
    }
}
