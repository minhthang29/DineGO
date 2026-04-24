using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Admin
    {
        [Key]
        public int ad_id { get; set; }

        [Required, MaxLength(50)]
        public string ad_username { get; set; }

        [Required, MaxLength(255)]
        public string ad_password { get; set; }

        [Required, MaxLength(255)]
        public string ad_name { get; set; }

        [Required, MaxLength(100)]
        public string ad_email { get; set; }

        [Required]
        public DateTime ad_birthday { get; set; }

        [MaxLength(255)]
        public string? ad_image { get; set; }

        public bool? ad_is_use { get; set; }
        public bool admin_is_deleted { get; set; }

        public ICollection<Voucher>? vouchers { get; set; }

        [JsonIgnore]
        public ICollection<SystemLog>? systemLogs { get; set; }
        public ICollection<Blog> blogs { get; set; }
    }
}
