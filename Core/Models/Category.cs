using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Category
    {
        [Key]
        public int cate_id { get; set; }

        [Required, MaxLength(50)]
        public string cate_type { get; set; }

        [MaxLength(200)]
        public string? cate_description { get; set; }
        public bool cate_is_deleted { get; set; }

        public ICollection<Restaurant>? restaurants { get; set; }
    }
}