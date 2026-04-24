using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Delivery
    {
        [Key]
        public int de_id { get; set; }

        public int order_id { get; set; }

        [Required]
        public int de_status { get; set; }

        [Required]
        public DateTime de_start { get; set; }

        [Required]
        public DateTime de_end { get; set; }

        [Required]
        public string de_note { get; set; }
        public bool de_is_deleted { get; set; }

        public Order? order { get; set; }
    }
}