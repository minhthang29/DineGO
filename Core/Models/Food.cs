using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Models
{
    public class Food
    {
        [Key]
        public int food_id { get; set; }

        public int menu_id { get; set; }

        [Required, MaxLength(100)]
        public string food_name { get; set; }

        [Required, MaxLength(200)]
        public string food_description { get; set; }

        [Required]
        public decimal food_price { get; set; }

        [Required, MaxLength(300)]
        public string food_image { get; set; }
        [NotMapped]
        public List<string> food_images
        {
            get
            {
                if (string.IsNullOrEmpty(food_image))
                    return new List<string>();

                try
                {
         
                    if (food_image.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(food_image) ?? new List<string>();

              
                    return food_image.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
        [Required]
        public int? food_status { get; set; }

        [MaxLength(200)]
        public string? food_tag { get; set; } // chuỗi tag phân cách bằng dấu phẩy
        public int? food_prep_time { get; set; }

        public bool? food_is_deleted { get; set; } = false;

        public Menu? menu { get; set; }

        public ICollection<FoodMenu>? food_menus { get; set; }
        [JsonIgnore]
        public ICollection<CartFood>? cart_foods { get; set; }
    }
}