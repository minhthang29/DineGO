
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;  // ← FIX: Import cho JsonPropertyName
using System.Collections.Generic;
using Core.Models;  // I

namespace Core.Models.Client.Custom
{
    public class CustomViewOrderDetailsDto
{
    [JsonPropertyName("order")]
    public Order Order { get; set; }  // Order chính từ DAO (có orderDetails bên trong nếu fallback)
    [JsonPropertyName("orderDetails")]
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();  // List chính từ DAO (2 items)
    [JsonPropertyName("listCartFood")]
    public List<CartFood> ListCartFood { get; set; } = new List<CartFood>();  // Từ DAO (có thể empty)
    [JsonPropertyName("listFood")]
    public List<Food> ListFood { get; set; } = new List<Food>();  // Từ DAO (có thể empty)
    [JsonPropertyName("voucher")]
    public object Voucher { get; set; }  // null từ JSON (optional, không có trong DAO)
    [JsonPropertyName("restaurant")]
    public Restaurant Restaurant { get; set; }  // Từ DAO (restaurant từ order)
}}