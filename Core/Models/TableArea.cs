using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Core.Models
{
    public class TableArea
    {
        [Key]
        public int area_id { get; set; }

        public int res_id { get; set; }

        [Required, MaxLength(100)]
        public string area_name { get; set; }

        [MaxLength(255)]
        public string? area_description { get; set; }

        public bool is_deleted { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.Now;

        public DateTime? updated_at { get; set; }

        [ForeignKey("res_id")]
        public Restaurant? restaurant { get; set; }
    }
}