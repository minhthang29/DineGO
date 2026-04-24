using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DineGO_Api.Data;
using Core.Models;
using Core.Models.Client;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DineGO_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestDashboard()
        {
            var stats = await _db.DashboardStats
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (stats == null)
                return NotFound();

            return Ok(stats);
        }
        [HttpGet("restaurant-owner/{resOwnerId}")]
        public async Task<IActionResult> GetDashboardResowner(int resOwnerId)
        {
            // Lấy danh sách nhà hàng của chủ sở hữu
            var restaurants = await _db.Restaurants.Where(r => r.res_owner_id == resOwnerId).ToListAsync();
            var restaurantIds = restaurants.Select(r => r.res_id).ToList();

            // Tổng số nhà hàng
            int totalRestaurants = restaurants.Count;

            // Tổng doanh thu
            decimal totalRevenue = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id))
                .SumAsync(o => (decimal?)o.order_total) ?? 0;

            // Tổng số đơn hàng
            int totalOrders = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id))
                .CountAsync();

            // Tổng số khách hàng
            int totalCustomers = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id))
                .Select(o => o.cus_id)
                .Distinct()
                .CountAsync();

            // Giao dịch thành công/thất bại
            int successTransactions = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id) && o.order_status == 3)
                .CountAsync();
            int failedTransactions = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id) && o.order_status == 4)
                .CountAsync();

            // Giấy phép đã xác thực/chưa
            int verifiedLicenses = await _db.Verifications
                .Where(v => restaurantIds.Contains(v.res_id) && v.ver_status == 1)
                .CountAsync();
            int unverifiedLicenses = await _db.Verifications
                .Where(v => restaurantIds.Contains(v.res_id) && v.ver_status != 1)
                .CountAsync();

            // Sao trung bình
            double averageRating = await _db.Restaurants
                .Where(r => restaurantIds.Contains(r.res_id))
                .AverageAsync(r => (double?)r.res_rate) ?? 0;

            // Thống kê theo tháng (7 tháng gần nhất)
            var months = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .Select(d => d.ToString("M/yyyy"))
                .ToList();

            var revenueData = new List<decimal>();
            var orderData = new List<int>();
            foreach (var month in months)
            {
                var dt = DateTime.ParseExact(month, "M/yyyy", null);
                var monthOrders = await _db.Orders
                    .Where(o => restaurantIds.Contains(o.res_id) && o.order_date.Month == dt.Month && o.order_date.Year == dt.Year)
                    .ToListAsync();
                revenueData.Add(monthOrders.Sum(o => (decimal)o.order_total));
                orderData.Add(monthOrders.Count);
            }

            // Top 5 nhà hàng doanh thu cao nhất
            var topRestaurantsQuery = await _db.Orders
                .Where(o => restaurantIds.Contains(o.res_id))
                .GroupBy(o => o.res_id)
                .Select(g => new
                {
                    res_id = g.Key,
                    revenue = g.Sum(x => (decimal)x.order_total)
                })
                .OrderByDescending(x => x.revenue)
                .Take(5)
                .ToListAsync();

            var topRestaurants = new List<string>();
            var topRestaurantRevenue = new List<decimal>();
            foreach (var item in topRestaurantsQuery)
            {
                var res = await _db.Restaurants.FindAsync(item.res_id);
                topRestaurants.Add(res?.res_name ?? $"ID {item.res_id}");
                topRestaurantRevenue.Add(item.revenue);
            }

            // Top 5 món ăn bán chạy nhất
            var topFoodsQuery = await (
                from od in _db.OrderDetails
                join o in _db.Orders on od.order_id equals o.order_id
                join cf in _db.CartFoods on od.cart_id equals cf.cart_id
                join f in _db.Foods on cf.food_id equals f.food_id
                where restaurantIds.Contains(o.res_id)
                group od by new { f.food_id, f.food_name } into g
                select new
                {
                    food_id = g.Key.food_id,
                    food_name = g.Key.food_name,
                    total = g.Sum(x => x.order_quantity)
                }
            )
            .OrderByDescending(x => x.total)
            .Take(5)
            .ToListAsync();

            var topFoods = topFoodsQuery.Select(x => x.food_name).ToList();
            var topFoodOrders = topFoodsQuery.Select(x => x.total).ToList();

            var result = new DashboardResownerViewModel
            {
                TotalRestaurants = totalRestaurants,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalCustomers = totalCustomers,
                SuccessTransactions = successTransactions,
                FailedTransactions = failedTransactions,
                VerifiedLicenses = verifiedLicenses,
                UnverifiedLicenses = unverifiedLicenses,
                AverageRating = averageRating,
                Months = months,
                RevenueData = revenueData,
                OrderData = orderData,
                TopRestaurants = topRestaurants,
                TopRestaurantRevenue = topRestaurantRevenue,
                TopFoods = topFoods,
                TopFoodOrders = topFoodOrders
            };

            return Ok(result);
        }
    }
}