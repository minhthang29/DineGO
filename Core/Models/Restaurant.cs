using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Restaurant
    {
        [Key]
        public int res_id { get; set; }

        public int cate_id { get; set; }
        public int res_owner_id { get; set; }

        [Required, MaxLength(500)]
        public string res_name { get; set; }

        [MaxLength(200)]
        public string? res_address { get; set; }

        [MaxLength(255)]
        public string? res_email { get; set; }

        [MaxLength(20)]
        public string? res_phone { get; set; }

        public string? res_description { get; set; }

        public decimal? res_rate { get; set; }
        public int res_rate_count { get; set; }
        public decimal? res_reservation_fee { get; set; } = 50000;
        public decimal? res_discount_promotion { get; set; }
        public decimal res_price_order_min { get; set; } = 200000;
        public decimal res_price_order_max { get; set; } = 5000000;
        public int res_quantity_order_max { get; set; } = 50;
        [JsonIgnore]
        public TimeSpan? res_open_time { get; set; }
        [JsonIgnore]
        public TimeSpan? res_close_time { get; set; }
        public int? res_last_order_minutes { get; set; } = 60;
        public int? res_meal_duration_minutes { get; set; } = 120;

        // Chỉ dùng để bind form (NotMapped = không lưu DB)
        [NotMapped]
        public string res_open_time_str
        {
            get => res_open_time.HasValue ? res_open_time.Value.ToString(@"hh\:mm") : null;
            set => res_open_time = string.IsNullOrEmpty(value) ? null : TimeSpan.Parse(value);
        }

        [NotMapped]
        public string res_close_time_str
        {
            get => res_close_time.HasValue ? res_close_time.Value.ToString(@"hh\:mm") : null;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    res_close_time = null;
                }
                else if (value == "24:00")
                {
                    // Gán trực tiếp 23:59
                    res_close_time = new TimeSpan(23, 59, 59);
                }
                else if (TimeSpan.TryParseExact(value, @"hh\:mm", null, out var ts))
                {
                    res_close_time = ts;
                }
                else
                {
                    res_close_time = null; // fallback tránh lỗi
                }
            }
        }

        [NotMapped]
        public List<string> AvailableTimeSlots
        {
            get
            {
                var slots = new List<string>();

                if (res_open_time == null || res_close_time == null)
                    return slots;

                var start = res_open_time.Value;
                var end = res_close_time.Value;

                for (var t = start; t < end; t = t.Add(TimeSpan.FromMinutes(30)))
                {
                    slots.Add(t.ToString(@"hh\:mm"));
                }

                return slots;
            }
        }

        public string? res_images { get; set; }

        [NotMapped]
        public List<string> res_images_json
        {
            get
            {
                if (string.IsNullOrEmpty(res_images))
                    return new List<string>();

                try
                {
                    // Nếu là chuỗi JSON
                    if (res_images.Trim().StartsWith("["))
                        return JsonSerializer.Deserialize<List<string>>(res_images) ?? new List<string>();

                    // Nếu là chuỗi phân cách dấu phẩy
                    return res_images.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        public bool res_is_use { get; set; }
        public bool res_is_authorized { get; set; }
        public bool res_is_deleted { get; set; }

        [NotMapped]
        public double distance_km { get; set; }

        public double? res_latitude { get; set; }
        public double? res_longitude { get; set; }
        public Category? category { get; set; }
        public RestaurantOwner? restaurantOwner { get; set; }
        [JsonIgnore]
        public ICollection<Reservation>? reservations { get; set; }
        public ICollection<RestaurantRating> restaurantRatings { get; set; }
        public ICollection<Menu>? menus { get; set; }
        [JsonIgnore]
        public ICollection<Order>? orders { get; set; }
        public ICollection<Post>? posts { get; set; }
        public ICollection<Cart>? carts { get; set; }
        public ICollection<Table> Tables { get; set; }
        public ICollection<Verification>? verifications { get; set; }
    }
}
