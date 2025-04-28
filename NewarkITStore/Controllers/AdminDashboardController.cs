using System.Text;
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
                : $"Showing stats from {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}";

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

        [HttpGet]
        public async Task<IActionResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;
            startDate ??= today.AddDays(-30);
            endDate ??= today;

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("OrderId,OrderDate,Product,Quantity,PricePerUnit,Total");

            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    csv.AppendLine($"{order.OrderId},{order.OrderDate:yyyy-MM-dd},{item.Product.Name},{item.Quantity},{item.PricePerUnit},{item.PricePerUnit * item.Quantity}");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"OrderStats_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
        }

        public async Task<IActionResult> TotalPerCard()
        {
            var data = await _context.Orders
                .Include(o => o.CreditCard)
                .Include(o => o.User)
                .Where(o => o.CreditCardId != null)
                .GroupBy(o => new { o.CreditCardId, o.CreditCard.CardNumber, o.User.Email })
                .Select(g => new CreditCardStatsViewModel
                {
                    UserEmail = g.Key.Email,
                    MaskedCardNumber = "**** **** **** " + g.Key.CardNumber.Substring(g.Key.CardNumber.Length - 4),
                    TotalCharged = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.TotalCharged)
                .ToListAsync();

            return View("TotalPerCard", data);
        }



    }
}
