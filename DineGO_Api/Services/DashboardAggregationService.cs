using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DineGO_Api.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Core.Models;

namespace DineGO_Api.Services
{
    public class DashboardStatsAggregationService
    {
        private readonly ApplicationDbContext _db;
        public DashboardStatsAggregationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AggregateAsync()
        {
            var now = DateTime.Now;
            var fromMonth = now.AddMonths(-11); // 12 tháng gần nhất

            // 1. Doanh thu theo tháng
            var revenueByMonthRaw = await _db.Orders
                .Where(o => o.order_date >= fromMonth)
                .GroupBy(o => new { o.order_date.Year, o.order_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Sum(x => x.order_total)
                })
                .ToListAsync();

            var revenueByMonth = revenueByMonthRaw
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new
                {
                    Label = $"{x.Month}/{x.Year}",
                    Value = x.Value
                })
                .ToList();

            // 2. Số lượng đơn hàng theo tháng
            var ordersByMonthRaw = await _db.Orders
                .Where(o => o.order_date >= fromMonth)
                .GroupBy(o => new { o.order_date.Year, o.order_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Count()
                })
                .ToListAsync();

            var ordersByMonth = ordersByMonthRaw
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new
                {
                    Label = $"{x.Month}/{x.Year}",
                    Value = x.Value
                })
                .ToList();

            // 3. Tỷ lệ loại khách hàng
            var customerType = await _db.Customers
                .GroupBy(c => (c.cus_is_kyc ?? false) ? "Đã KYC" : "Chưa KYC")
                .Select(g => new { Label = g.Key, Value = g.Count() })
                .ToListAsync();

            // 4. Tỷ lệ trạng thái đơn hàng
            var orderStatusRaw = await _db.Orders
                .GroupBy(o => o.order_status)
                .Select(g => new { Status = g.Key, Value = g.Count() })
                .ToListAsync();

            var orderStatus = orderStatusRaw
                .Select(x => new
                {
                    Label = x.Status switch
                    {
                        0 => "Chờ xác nhận",
                        1 => "Đã xác nhận",
                        2 => "Đang giao",
                        3 => "Hoàn thành",
                        4 => "Đã hủy",
                        _ => "Khác"
                    },
                    Value = x.Value
                })
                .ToList();

            // 5. Top 5 nhà hàng doanh thu cao
            var topRestaurants = await _db.Orders
                .Where(o => o.restaurant != null)
                .GroupBy(o => o.restaurant.res_name)
                .Select(g => new { Label = g.Key, Value = g.Sum(x => x.order_total) })
                .OrderByDescending(x => x.Value)
                .Take(5)
                .ToListAsync();

            // 6. So sánh doanh thu các dịch vụ (Order, Reservation)
            var reservationRevenueRaw = await _db.Reservations
                .Where(r => r.reser_date >= fromMonth)
                .GroupBy(r => new { r.reser_date.Year, r.reser_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            var reservationRevenueLabels = reservationRevenueRaw
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => $"{x.Month}/{x.Year}")
                .ToList();

            var reservationRevenueValues = await _db.Payments
                .Where(p => p.reservation != null && p.pay_created_date >= fromMonth)
                .GroupBy(p => new { p.pay_created_date.Year, p.pay_created_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Value = g.Sum(x => x.pay_price)
                })
                .ToListAsync();

            var reservationRevenueValuesOrdered = reservationRevenueValues
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => x.Value)
                .ToList();

            var serviceRevenue = new[]
            {
                new {
                    Service = "Order",
                    Labels = revenueByMonth.Select(x => x.Label).ToList(),
                    Values = revenueByMonth.Select(x => x.Value).ToList()
                },
                new {
                    Service = "Reservation",
                    Labels = reservationRevenueLabels,
                    Values = reservationRevenueValuesOrdered
                }
            };

            // 7. Phân tích khách hàng theo nhóm (ví dụ theo giới tính)
            var customerGroup = await _db.Customers
                .GroupBy(c => (c.cus_gender ?? false) ? "Nam" : "Nữ")
                .Select(g => new { Label = g.Key, Value = g.Count() })
                .ToListAsync();

            // Lưu vào DashboardStats
            var stats = new DashboardStats
            {
                ReportDate = now,
                RevenueByMonthJson = JsonSerializer.Serialize(new
                {
                    labels = revenueByMonth.Select(x => x.Label),
                    values = revenueByMonth.Select(x => x.Value)
                }),
                OrdersByMonthJson = JsonSerializer.Serialize(new
                {
                    labels = ordersByMonth.Select(x => x.Label),
                    values = ordersByMonth.Select(x => x.Value)
                }),
                CustomerTypeJson = JsonSerializer.Serialize(new
                {
                    labels = customerType.Select(x => x.Label),
                    values = customerType.Select(x => x.Value)
                }),
                OrderStatusJson = JsonSerializer.Serialize(new
                {
                    labels = orderStatus.Select(x => x.Label),
                    values = orderStatus.Select(x => x.Value)
                }),
                TopRestaurantJson = JsonSerializer.Serialize(new
                {
                    labels = topRestaurants.Select(x => x.Label),
                    values = topRestaurants.Select(x => x.Value)
                }),
                ServiceRevenueJson = JsonSerializer.Serialize(serviceRevenue),
                CustomerGroupJson = JsonSerializer.Serialize(new
                {
                    labels = customerGroup.Select(x => x.Label),
                    values = customerGroup.Select(x => x.Value)
                }),
                CreatedAt = now
            };

            _db.DashboardStats.Add(stats);
            await _db.SaveChangesAsync();
        }
    }
}