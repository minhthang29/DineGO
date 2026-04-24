using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Core.Models.Client.Custom;
using System.Text.Json;  // Cho JsonException
using System.Linq;  // Cho Any(), FirstOrDefault()
using System.Net.Http;
using Core.Models.Client.Custom;

namespace DineGO_Client.Controllers
{
    public class OrderController : BaseController // Kế thừa Controller
    {
        private readonly ApiService _apiService;
        private readonly CustomerPointService _pointService;

        public OrderController(ApiService apiService, CustomerPointService pointService)
        {
            _apiService = apiService;
            _pointService = pointService;
        }

        public async Task<IActionResult> Index()
        {

            var orders = await _apiService.GetAsync<List<Order>>($"Order/restaurant/{resId.Value}");
            // Sắp xếp theo ngày mới nhất
            orders = orders.OrderByDescending(o => o.order_date).ToList();
            foreach (var o in orders)
            {
                o.customer = await _apiService.GetAsync<Customer>($"Customer/{o.cus_id}");
            }
            ViewBag.RestaurantId = resId.Value;
            return View(orders);
        }

        public async Task<IActionResult> Detail(int id)
{
    try
    {
        // Gọi API như cũ, deserialize thành DTO (match backend DAO structure)
        var response = await _apiService.GetAsync<CustomViewOrderDetailsDto>($"Order/details/{id}");
        if (response == null)
            return NotFound($"Đơn hàng ID={id} không tồn tại.");
        // Extract List<OrderDetail> từ DTO (từ backend DAO's OrderDetails)
        var orderDetails = response.OrderDetails ?? response.Order?.orderDetails ?? new List<OrderDetail>();
        if (orderDetails == null || !orderDetails.Any())
            return NotFound("Không có chi tiết đơn hàng từ DAO.");
        // Truyền vào view (List<OrderDetail> với navigation order populated)
        return View(orderDetails);
    }
    catch (JsonException ex)
    {
        // Catch lỗi deserialization (nếu JSON thay đổi)
        TempData["Error"] = "Lỗi phân tích dữ liệu từ API. Vui lòng thử lại.";
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Lỗi: {ex.Message}";
        return RedirectToAction("Index");
    }
}

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, int newStatus, int res_id)
        {
            var order = await _apiService.GetAsync<Order>($"Order/id?ID={orderId}");
            if (order == null) return NotFound();
            order.order_status = newStatus;
            order.customer = null;
            await _apiService.PutAsync<object, dynamic>($"Order", order);

            // ✅ Cộng điểm khi hoàn tất (status = 3)
            if (newStatus == 3)
            {
                var rawPoints = (int)(order.order_total * 0.01m); // 100k = 1000 điểm
                var bonusPoints = (int)(rawPoints * 0.01m);        // chỉ lấy 1%

                await _pointService.UpdatePointsAsync(new CustomerPointRequest
                {
                    CusId = order.cus_id,
                    ChangeAmount = bonusPoints,
                    Description = $"Hoàn tất đơn hàng +{bonusPoints} điểm"
                });
            }

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index", new { res_id });
        }
    }
}
