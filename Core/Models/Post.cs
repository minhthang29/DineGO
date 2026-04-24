using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

namespace Core.Models
{
    public class Post
    {
        [Key]
        public int post_id { get; set; }

        public int? res_id { get; set; }
        public int? cus_id { get; set; }

        [Required]
        public string post_content { get; set; }

        public string? post_image { get; set; }
        public string? post_video { get; set; }

        [NotMapped]
        public List<string>? post_images_json
        {
            get
            {
                if (string.IsNullOrEmpty(post_image))
                    return new List<string>();

                try
                {
                    // Nếu là chuỗi JSON
                    if (post_image.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(post_image) ?? new List<string>();

                    // Nếu là chuỗi phân cách dấu phẩy
                    return post_image.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        public DateTime post_created_date { get; set; }
        public DateTime post_updated_date { get; set; }

        [MaxLength(255)]
        public string? post_title { get; set; }

        [MaxLength(50)]
        public string? post_author_name { get; set; }

        public int? post_like_count { get; set; }
        public int? post_comment_count { get; set; }
        public bool post_is_approve { get; set; } = false;

        public Customer? customer { get; set; }
        public Restaurant? restaurant { get; set; }

        public ICollection<Comment>? comments { get; set; }
        public ICollection<Like>? likes { get; set; }
    }
}