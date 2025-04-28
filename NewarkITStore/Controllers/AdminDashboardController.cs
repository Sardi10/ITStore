using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.ViewModels;

namespace NewarkITStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;
            startDate ??= today.AddDays(-30); // default: 30 days ago
            endDate ??= today;

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.RangeMessage = (startDate == today.AddDays(-30) && endDate == today)
                ? "Showing stats for the last 30 days"
                : $"Showing stats from {startDate:MMM dd yyyy} to {endDate:MMM dd yyyy}";

            var ordersInRange = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            var totalRevenue = ordersInRange.Sum(o => o.TotalAmount);
            var totalOrders = ordersInRange.Count;

            var totalUsers = await _context.Users.CountAsync();

            var topProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new AdminDashboardViewModel.TopProduct
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(p => p.QuantitySold)
                .Take(5)
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalUsers = totalUsers,
                TopSellingProducts = topProducts
            };

            return View(model);
        }

    }
}
