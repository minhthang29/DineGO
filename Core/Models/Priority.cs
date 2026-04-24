using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Priority
    {
        [Key]
        public int priority_id { get; set; }

        public int cus_id { get; set; }

        [Required, MaxLength(50)]
        public string tag { get; set; } // VD: "mon_cay", "mon_chien"
        public int count { get; set; } = 1; // số lần khách tìm tag này
        public int click_count { get; set; } = 0; // Số lần click/chọn thực sự
        public DateTime? last_used { get; set; }
        public double score { get; set; } = 0;
        public double? weight_manual { get; set; }
        public Customer? customer { get; set; }
    }
}