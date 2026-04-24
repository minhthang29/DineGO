using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Verification
    {
        [Key]
        public int ver_id { get; set; }

        public int res_id { get; set; }

        [Required, MaxLength(100)]
        public string ver_license { get; set; }

        [Required, MaxLength(50)]
        public string ver_tax_code { get; set; }

        [Required, MaxLength(300)]
        public string ver_document { get; set; }

        [Required]
        public int ver_status { get; set; }

        public DateTime ver_date_submitted { get; set; }
        public DateTime? ver_date_verified { get; set; }

        [MaxLength(500)]
        public string? ver_file_attachment { get; set; }
        public bool ver_is_deleted { get; set; }
        public DateTime? ver_date_responded { get; set; }
        [MaxLength(500)]
        public string? ver_content_responded { get; set; }

        public Restaurant? restaurant { get; set; }
    }
}