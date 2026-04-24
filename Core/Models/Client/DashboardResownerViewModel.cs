using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Client
{
    public class DashboardResownerViewModel
    {
        public int TotalRestaurants { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int SuccessTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public int VerifiedLicenses { get; set; }
        public int UnverifiedLicenses { get; set; }
        public double AverageRating { get; set; }
        public List<string> Months { get; set; }
        public List<decimal> RevenueData { get; set; }
        public List<int> OrderData { get; set; }
        public List<string> TopRestaurants { get; set; }
        public List<decimal> TopRestaurantRevenue { get; set; }
        public List<string> TopFoods { get; set; }
        public List<int> TopFoodOrders { get; set; }
    }
}