using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class CustomerPointHistory
    {
        [Key]
        public int history_id { get; set; }

        [Required]
        public int point_id { get; set; }

        [Required]
        public int change_amount { get; set; }

        [Required]
        public int balance_after { get; set; }

        public string? description { get; set; }
        public DateTime created_date { get; set; }

        // Quan hệ
        public CustomerPoint? customerPoint { get; set; }
    }
}
