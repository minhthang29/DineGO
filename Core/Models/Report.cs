using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Report
    {
        [Key]
        public int report_id { get; set; }

        // Người báo cáo
        [ForeignKey("customer")]
        public int cus_id { get; set; }
        public Customer customer { get; set; }

        // Người bị báo cáo (có thể là customer hoặc owner)
        public int? reported_user_id { get; set; }

        [MaxLength(50)]
        public string report_type { get; set; }

        public string report_content { get; set; }

        public int report_status { get; set; }

        public DateTime report_created_at { get; set; }

        // Admin xử lý (có thể null nếu chưa xử lý)
        [ForeignKey("admin")]
        public int? admin_id { get; set; }
        public Admin? admin { get; set; }

        public string? admin_note { get; set; }

        [MaxLength(255)]
        public string? report_related_url { get; set; }
        public bool report_is_deleted { get; set; }
    }
}