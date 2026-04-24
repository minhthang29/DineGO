using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Reservation
    {
        [Key]
        public int reser_id { get; set; }

        public int? cus_id { get; set; }
        [Required]
        public int res_id { get; set; }
        [Required]
        public int table_id { get; set; }

        [Required]
        public DateTime reser_date { get; set; }

        [Required]
        public int reser_status { get; set; }

        public string? reser_note { get; set; }
        public bool reser_is_deleted { get; set; }
        [Required]
        public DateTime reser_create_at  { get; set; }

        public Customer? customer { get; set; }
        public Restaurant? restaurant { get; set; }
        [ForeignKey("table_id")]
        public Table? table { get; set; } = null!;

        public ICollection<Payment>? payments { get; set; }
    }
}