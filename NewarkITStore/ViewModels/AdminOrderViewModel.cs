using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NewarkITStore.Models;
using static NewarkITStore.Models.Order;

namespace NewarkITStore.ViewModels
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string UserEmail { get; set; } // Optional: If you want to display customer email
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderItemDetail> Items { get; set; } = new();
        public ICollection<OrderItem> OrderItems { get; set; }
        public string ShippingAddressSummary { get; set; } // e.g., "Home - NYC, NY"
        public string MaskedCard { get; set; } // e.g., "**** **** **** 1234"

        public decimal Total => Items.Sum(i => i.PricePerUnit * i.Quantity);
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }

    public class OrderItemDetail
    {
        public string ProductName { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }

        public decimal LineTotal => PricePerUnit * Quantity;
    }
}
