using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Order
    {
        [Key]
        public int order_id { get; set; }
        
        public int cus_id { get; set; }
        public int res_id { get; set; }
        
        [Required]
        public DateTime order_date { get; set; }
        
        [Required]
        public int order_status { get; set; }
        
        [Required]
        public decimal order_total { get; set; }
        
        // 👇 Thêm các trường để lưu thông tin cố định
        public decimal order_subtotal { get; set; }           // Tổng tiền món ăn (trước khi giảm giá)
        public decimal? delivery_fee { get; set; }            // Phí giao hàng cố định
        public decimal? order_price_discount { get; set; }    // Số tiền được giảm từ voucher
        
        [MaxLength(200)]
        public string? voucher_code_applied { get; set; }     // Mã voucher đã áp dụng
        public int? voucher_type_applied { get; set; }        // Loại voucher (0=%, 1=fixed)
        public decimal? voucher_original_value { get; set; }  // Giá trị gốc của voucher
        
        // Thông tin giao hàng cố định
        public DateTime? estimated_delivery_time { get; set; } // Thời gian giao hàng dự kiến
        public string? delivery_address_snapshot { get; set; } // Địa chỉ giao hàng tại thời điểm đặt
        public string? customer_phone_snapshot { get; set; }   // SĐT khách hàng tại thời điểm đặt
        public string? customer_name_snapshot { get; set; }    // Tên khách hàng tại thời điểm đặt
        
        public bool order_is_deleted { get; set; }
        
        // Relationships
        public Customer? customer { get; set; }
        public Restaurant? restaurant { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetail>? orderDetails { get; set; }
        public ICollection<Delivery>? delivery { get; set; }
    }
}