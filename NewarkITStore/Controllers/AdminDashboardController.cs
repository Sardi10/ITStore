using System.Globalization;
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

            startDate ??= today.AddDays(-30); // Default: last 30 days
            endDate ??= today;

            var query = _context.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.OrderDate <= endOfDay);
            }

            var orders = await query.ToListAsync();

            var viewModel = new SalesStatisticsViewModel
            {
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                NumberOfOrders = orders.Count,
                AverageOrderValue = orders.Count > 0 ? orders.Average(o => o.TotalAmount) : 0,
                NumberOfUniqueCustomers = orders.Select(o => o.User.Id).Distinct().Count(),
                DailyRevenues = orders
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new DailyRevenueData
                    {
                        Date = g.Key,
                        Revenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderBy(d => d.Date)
                    .ToList()
            };

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(viewModel);
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

        public async Task<IActionResult> TotalPerCard(string? email, string? cardLast4, decimal? minAmount, decimal? maxAmount, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Orders
            .Include(o => o.CreditCard)
            .Include(o => o.User)
             .Where(o => o.CreditCardId != null);

            if (!string.IsNullOrEmpty(email))
                query = query.Where(o => o.User.Email.Contains(email));

            if (!string.IsNullOrEmpty(cardLast4))
                query = query.Where(o => o.CreditCard.CardNumber.EndsWith(cardLast4));

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1); // Sets time to 23:59:59.9999999
                query = query.Where(o => o.OrderDate <= endOfDay);
            }
            var grouped = await query
                .GroupBy(o => new { o.CreditCardId, o.CreditCard.CardNumber, o.User.Email })
                .Select(g => new CreditCardStatsViewModel
                {
                    UserEmail = g.Key.Email,
                    MaskedCardNumber = "**** **** **** " + g.Key.CardNumber.Substring(g.Key.CardNumber.Length - 4),
                    TotalCharged = g.Sum(o => o.TotalAmount)
                })
                .Where(x => (!minAmount.HasValue || x.TotalCharged >= minAmount.Value)
                         && (!maxAmount.HasValue || x.TotalCharged <= maxAmount.Value))
                .OrderByDescending(x => x.TotalCharged)
                .ToListAsync();

            return View("TotalPerCard", grouped);
        }

        public async Task<IActionResult> TopCustomers(DateTime? startDate,DateTime? endDate,string productName,string productCategory,int topN = 10)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value.Date.AddDays(1).AddSeconds(-1));

            if (!string.IsNullOrEmpty(productName))
                query = query.Where(o => o.OrderItems.Any(oi => oi.Product.Name.Contains(productName)));

            if (!string.IsNullOrEmpty(productCategory))
                query = query.Where(o => o.OrderItems.Any(oi => oi.Product.ProductType.Name.Contains(productCategory)));

            var result = await query
                .SelectMany(o => o.OrderItems.Select(oi => new
                {
                    o.User.Email,
                    Amount = oi.PricePerUnit * oi.Quantity * 1.1m // assuming 10% tax
                }))
                .GroupBy(x => x.Email)
                .Select(g => new TopCustomerViewModel
                {
                    UserEmail = g.Key,
                    TotalSpent = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(topN)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> TopSellingProducts(DateTime? startDate, DateTime? endDate, string category, int topN = 10, string sortBy = "revenue")
        {
            var query = _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value.Date.AddDays(1).AddSeconds(-1));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(oi => oi.Product.ProductType.Name.ToLower().Contains(category.ToLower()));

            var data = await query
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new TopProductStatsViewModel
                {
                    ProductName = g.Key,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.PricePerUnit * 1.1m) // 10% tax
                })
                .ToListAsync();

            // Sort based on selected option
            data = sortBy.ToLower() == "units"
                ? data.OrderByDescending(x => x.UnitsSold).Take(topN).ToList()
                : data.OrderByDescending(x => x.TotalRevenue).Take(topN).ToList();

            // Pass sortBy to View
            ViewBag.SortBy = sortBy;
            ViewBag.TopN = topN;

            return View(data);
        }


        public async Task<IActionResult> TopProductsByCustomers(DateTime? startDate, DateTime? endDate, int topN = 10)
        {
            var query = _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                    .ThenInclude(o => o.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate <= endDate.Value.Date.AddDays(1).AddSeconds(-1));

            var result = await query
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new TopProductByCustomersViewModel
                {
                    ProductName = g.Key,
                    UniqueCustomerCount = g.Select(oi => oi.Order.User.Id).Distinct().Count()
                })
                .OrderByDescending(x => x.UniqueCustomerCount)
                .Take(topN)
                .ToListAsync();

            return View(result);
        }

        public async Task<IActionResult> MaxBasketPerCard(DateTime? startDate, DateTime? endDate, int topN = 10)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.CreditCard)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value.Date.AddDays(1).AddSeconds(-1));

            var data = await query
                .GroupBy(o => new { o.User.Email, o.CreditCard.CardNumber })
                .Select(g => new MaxBasketPerCardViewModel
                {
                    UserEmail = g.Key.Email,
                    MaskedCardNumber = "**** **** **** " + g.Key.CardNumber.Substring(g.Key.CardNumber.Length - 4),
                    MaxBasketTotal = g.Max(o => o.OrderItems.Sum(oi => oi.PricePerUnit * oi.Quantity * 1.1m)) // assuming 10% tax
                })
                .OrderByDescending(x => x.MaxBasketTotal)
                .Take(topN)
                .ToListAsync();

            return View(data);
        }

        public async Task<IActionResult> AveragePriceByType(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(oi => oi.Order.OrderDate >= startDate.Value);

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(oi => oi.Order.OrderDate <= endOfDay);
            }

            var data = await query
               .GroupBy(oi => oi.Product.ProductType.Name)
                .Select(g => new AveragePriceByProductTypeViewModel
                {
                    ProductType = g.Key,
                    AveragePrice = g.Average(oi => oi.PricePerUnit)
                })
                .OrderByDescending(x => x.AveragePrice)
                .ToListAsync();

            return View(data);
        }


    }
}
