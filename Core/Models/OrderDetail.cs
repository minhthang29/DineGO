using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
namespace Core.Models
{
    public class OrderDetail
    {
        [Key]
        public int order_detail_id { get; set; }
        
        public int? cart_id { get; set; }
        public int order_id { get; set; }
        
        [Required]
        public int order_quantity { get; set; }
        
        [Required]
        public decimal order_price { get; set; }              // Giá tại thời điểm đặt hàng
        
        // 👇 Thêm các trường để lưu thông tin món ăn cố định
        public int food_id { get; set; }                      // ID món ăn
        public string food_name_snapshot { get; set; }        // Tên món ăn tại thời điểm đặt
        public decimal food_price_snapshot { get; set; }      // Giá món ăn tại thời điểm đặt
        public string? food_description_snapshot { get; set; } // Mô tả món ăn
        public string? food_image_snapshot { get; set; }      // Ảnh món ăn tại thời điểm đặt
        // 👇 Property không map để parse images
        [NotMapped]
        public List<string> FoodImageList
        {
            get
            {
                if (string.IsNullOrEmpty(food_image_snapshot))
                    return new List<string>();

                try
                {
                    // Nếu là JSON array
                    if (food_image_snapshot.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(food_image_snapshot) ?? new List<string>();

                    // Nếu là chuỗi phân cách bằng dấu phay
                    return food_image_snapshot.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }
        
        // 👇 Property để lấy ảnh đầu tiên (tiện dùng)
        [NotMapped]
        public string? FirstImage
        {
            get
            {
                var images = FoodImageList;
                return images.Any() ? images.First() : null;
            }
        }
        public int? prep_time_snapshot { get; set; }          // Thời gian chế biến tại thời điểm đặt
        
        // Relationships
        public Order? order { get; set; }
        public Cart? cart { get; set; }
    }
}