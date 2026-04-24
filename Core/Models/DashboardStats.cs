using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class DashboardStats
    {
        [Key]
        public int Id { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;

        // 1. Doanh thu theo tháng (labels, values)
        public string RevenueByMonthJson { get; set; } // {labels:[], values:[]}

        // 2. Số lượng đơn hàng theo tháng (labels, values)
        public string OrdersByMonthJson { get; set; }

        // 3. Tỷ lệ loại khách hàng (labels, values)
        public string CustomerTypeJson { get; set; }

        // 4. Tỷ lệ trạng thái đơn hàng (labels, values)
        public string OrderStatusJson { get; set; }

        // 5. Top 5 nhà hàng doanh thu cao (labels, values)
        public string TopRestaurantJson { get; set; }

        // 6. So sánh doanh thu các dịch vụ (labels, datasets)
        public string ServiceRevenueJson { get; set; }

        // 7. Phân tích khách hàng theo nhóm (labels, values)
        public string CustomerGroupJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}