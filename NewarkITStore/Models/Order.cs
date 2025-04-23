using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewarkITStore.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } // FK to ApplicationUser
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public ICollection<OrderItem> OrderItems { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
