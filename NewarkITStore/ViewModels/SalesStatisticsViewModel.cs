using System;
using System.Collections.Generic;

namespace NewarkITStore.ViewModels
{
    public class SalesStatisticsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int NumberOfOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int NumberOfUniqueCustomers { get; set; }

        // Daily Revenue for Chart
        public List<DailyRevenueData> DailyRevenues { get; set; } = new List<DailyRevenueData>();
    }

    public class DailyRevenueData
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }
}
