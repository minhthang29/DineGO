using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Services;
using Core.Constant;
using System.Text.Json; 
using System;

namespace DineGO_Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApiService _apiService;

        public OrderController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _apiService.GetAsync<List<Order>>(ApiEndpoints.ORDER);
            foreach (var o in orders)
            {
                o.customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{o.cus_id}");
                o.restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{o.res_id}");
            }
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, int newStatus)
        {
            var order = await _apiService.GetAsync<Order>($"{ApiEndpoints.ORDER}/id?ID={orderId}");

            if (order == null)
                return NotFound();

            order.order_status = newStatus;
            order.customer = null;
            await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.ORDER}", order);

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Sử dụng endpoint đúng như API Controller
                var order = await _apiService.GetAsync<Order>($"{ApiEndpoints.ORDER}/id?ID={id}");
                
                if (order == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                    return RedirectToAction("Index");
                }

                // Load customer data
                if (order.cus_id > 0)
                {
                    try
                    {
                        order.customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{order.cus_id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading customer {order.cus_id}: {ex.Message}");
                    }
                }

                // Load restaurant data
                if (order.res_id > 0)
                {
                    try
                    {
                        order.restaurant = await _apiService.GetAsync<Restaurant>($"{ApiEndpoints.RESTAURANT_BY_ID}{order.res_id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading restaurant {order.res_id}: {ex.Message}");
                    }
                }

                // Load order details using the correct API endpoint
                try
                {
                    // Sử dụng endpoint đã có trong API: /details/{orderId}
                    var orderDetails = await _apiService.GetAsync<List<OrderDetail>>($"{ApiEndpoints.ORDER}/details/{id}");
                    
                    if (orderDetails != null && orderDetails.Count > 0)
                    {
                        order.orderDetails = orderDetails;
                        
                        // Không cần load thêm menu item data vì đã có snapshot
                        // Thông tin món ăn đã được lưu trong OrderDetail dưới dạng snapshot
                    }
                    else
                    {
                        order.orderDetails = new List<OrderDetail>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading order details for order {id}: {ex.Message}");
                    order.orderDetails = new List<OrderDetail>();
                }

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}