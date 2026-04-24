using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Comment
    {
        [Key]
        public int comment_id { get; set; }

        public int post_id { get; set; }
        public int cus_id { get; set; }

        [Required]
        public string comment_content { get; set; }

        public DateTime comment_created_date { get; set; }
        public DateTime comment_updated_date { get; set; }

        public Post? post { get; set; }
        public Customer? customer { get; set; }
    }
}