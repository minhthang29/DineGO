using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client
{
    public class CreateOrderWithDetailsRequest
    {
        public int cus_id { get; set; }
        public int res_id { get; set; }
        public DateTime order_date { get; set; }
        public int order_status { get; set; }
        public decimal order_subtotal { get; set; }
        public decimal? delivery_fee { get; set; }
        public decimal? order_price_discount { get; set; }
        public string? voucher_code_applied { get; set; }
        public int? voucher_type_applied { get; set; }
        public decimal? voucher_original_value { get; set; }
        public decimal order_total { get; set; }
        public DateTime? estimated_delivery_time { get; set; }
        public string? customer_name_snapshot { get; set; }
        public string? customer_phone_snapshot { get; set; }
        public string? delivery_address_snapshot { get; set; }

        public List<OrderDetailRequest> OrderDetails { get; set; } = new();
    }
     public class OrderDetailRequest
    {
        public int? cart_id { get; set; }
        public int food_id { get; set; }
        public int order_quantity { get; set; }
        public decimal order_price { get; set; }
        public string food_name_snapshot { get; set; } = string.Empty;
        public decimal food_price_snapshot { get; set; }
        public string? food_image_snapshot { get; set; }
        public int? prep_time_snapshot { get; set; }
    }
}