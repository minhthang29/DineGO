using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

namespace Core.Models
{
    public class Table
    {
        [Key]
        public int table_id { get; set; }

        [Required]
        public int res_id { get; set; } // Foreign Key đến Restaurant

        [Required]
        [StringLength(100)]
        public string table_name { get; set; }

        [Required]
        public int area_id { get; set; }

        [Required]
        public int table_seat { get; set; } // Số ghế (2, 4, 6...)

        [StringLength(255)]
        public string? table_image { get; set; } // Link ảnh bàn (nếu có)

        [NotMapped]
        public List<string> table_image_json
        {
            get
            {
                if (string.IsNullOrEmpty(table_image))
                    return new List<string>();

                try
                {
                    // Nếu là chuỗi JSON
                    if (table_image.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(table_image) ?? new List<string>();

                    // Nếu là chuỗi phân cách dấu phẩy
                    return table_image.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        [Required]
        public int table_position_x { get; set; }

        [Required]
        public int table_position_y { get; set; }

        public bool table_is_deleted { get; set; } = false;

        public DateTime table_created_at { get; set; } = DateTime.Now;

        public DateTime? table_update_at { get; set; }
        [Required]
        public int table_status { get; set; } = 0;

        public Restaurant? Restaurant { get; set; }
        [ForeignKey("area_id")]
        public TableArea? TableArea { get; set; }


    }
}
