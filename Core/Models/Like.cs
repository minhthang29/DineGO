using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Like
    {
        [Key]
        public int like_id { get; set; }

        public int? post_id { get; set; }
        public int? cus_id { get; set; }
        public int? like_emotion_type { get; set; }

        public Post? post { get; set; }
        public Customer? customer { get; set; }
    }

}