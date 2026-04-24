using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.Models;
using Core.Constant;
using System.Text.Json;

namespace DineGO_Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.RevenueByMonthJson))
                return NotFound();
            var revenue = JsonSerializer.Deserialize<RevenueByMonthModel>(stats.RevenueByMonthJson);
            return Ok(revenue);
        }

        [HttpGet("orders-by-month")]
        public async Task<IActionResult> GetOrdersByMonth()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.OrdersByMonthJson))
                return NotFound();
            var orders = JsonSerializer.Deserialize<OrdersByMonthModel>(stats.OrdersByMonthJson);
            return Ok(orders);
        }

        [HttpGet("customer-type")]
        public async Task<IActionResult> GetCustomerType()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.CustomerTypeJson))
                return NotFound();
            var customerType = JsonSerializer.Deserialize<CustomerTypeModel>(stats.CustomerTypeJson);
            return Ok(customerType);
        }

        [HttpGet("order-status")]
        public async Task<IActionResult> GetOrderStatus()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.OrderStatusJson))
                return NotFound();
            var orderStatus = JsonSerializer.Deserialize<OrderStatusModel>(stats.OrderStatusJson);
            return Ok(orderStatus);
        }

        [HttpGet("top-restaurant")]
        public async Task<IActionResult> GetTopRestaurant()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.TopRestaurantJson))
                return NotFound();
            var topRestaurant = JsonSerializer.Deserialize<TopRestaurantModel>(stats.TopRestaurantJson);
            return Ok(topRestaurant);
        }

        [HttpGet("service-revenue")]
        public async Task<IActionResult> GetServiceRevenue()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.ServiceRevenueJson))
                return NotFound();
            var serviceRevenue = JsonSerializer.Deserialize<ServiceRevenueModel[]>(stats.ServiceRevenueJson);
            return Ok(serviceRevenue);
        }

        [HttpGet("customer-group")]
        public async Task<IActionResult> GetCustomerGroup()
        {
            var stats = await _apiService.GetAsync<DashboardStats>($"{ApiEndpoints.DASHBOARD}/latest");
            if (stats == null || string.IsNullOrEmpty(stats.CustomerGroupJson))
                return NotFound();
            var customerGroup = JsonSerializer.Deserialize<CustomerGroupModel>(stats.CustomerGroupJson);
            return Ok(customerGroup);
        }

        // Models phụ trợ để parse JSON
        public class RevenueByMonthModel
        {
            public string[] labels { get; set; }
            public decimal[] values { get; set; }
        }
        public class OrdersByMonthModel
        {
            public string[] labels { get; set; }
            public int[] values { get; set; }
        }
        public class CustomerTypeModel
        {
            public string[] labels { get; set; }
            public int[] values { get; set; }
        }
        public class OrderStatusModel
        {
            public string[] labels { get; set; }
            public int[] values { get; set; }
        }
        public class TopRestaurantModel
        {
            public string[] labels { get; set; }
            public decimal[] values { get; set; }
        }
        public class ServiceRevenueModel
        {
            public string Service { get; set; }
            public string[] Labels { get; set; }
            public decimal[] Values { get; set; }
        }
        public class CustomerGroupModel
        {
            public string[] labels { get; set; }
            public int[] values { get; set; }
        }
    }
}