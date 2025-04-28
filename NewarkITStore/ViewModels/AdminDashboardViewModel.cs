using System;
using System.Collections.Generic;

namespace NewarkITStore.ViewModels
{
    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }

        public List<TopProduct> TopSellingProducts { get; set; }

        public class TopProduct
        {
            public string ProductName { get; set; }
            public int QuantitySold { get; set; }
        }
    }
}
