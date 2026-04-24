using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Models
{
    public class Menu
    {
        [Key]
        public int menu_id { get; set; }

        public int res_id { get; set; }

        [Required, MaxLength(20)]
        public string menu_type { get; set; }

        [Required, MaxLength(300)]
        public string menu_image { get; set; }

        [NotMapped]
        public List<string> menu_images
        {
            get
            {
                if (string.IsNullOrEmpty(menu_image))
                    return new List<string>();

                try
                {
                    if (menu_image.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(menu_image) ?? new List<string>();

                    return menu_image.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
        [Required, MaxLength(20)]
        public string menu_name { get; set; }

        public bool menu_is_deleted { get; set; } = false;

        public Restaurant? restaurant { get; set; }

        public ICollection<Food>? foods { get; set; }
        public ICollection<FoodMenu>? food_menus { get; set; }
    }
}