using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class SystemLog
    {
        [Key]
        public int sys_log_id { get; set; }

        public int? ad_id { get; set; }

        [MaxLength(100)]
        public string? action { get; set; }

        public string? description { get; set; }

        public DateTime? log_time { get; set; }

        [MaxLength(45)]
        public string? ip_address { get; set; }

        [MaxLength(255)]
        public string? device_info { get; set; }

        public int? status_code { get; set; }
        public bool? is_success { get; set; }

        public Admin? admin { get; set; }
    }
}