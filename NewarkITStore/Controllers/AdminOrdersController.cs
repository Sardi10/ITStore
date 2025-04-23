using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;
using NewarkITStore.ViewModels;
using static NewarkITStore.Models.Order;

namespace NewarkITStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AdminOrders
        public async Task<IActionResult> Index(string searchEmail, OrderStatus? filterStatus)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchEmail))
            {
                query = query.Where(o => o.User.Email.Contains(searchEmail));
            }

            if (filterStatus.HasValue)
            {
                query = query.Where(o => o.Status == filterStatus.Value);
            }

            var orders = await query.Select(o => new AdminOrderViewModel
            {
                OrderId = o.OrderId,
                UserEmail = o.User.Email,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDetail
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    PricePerUnit = oi.PricePerUnit
                }).ToList()
            }).ToListAsync();

            return View(orders);
        
    }


        // GET: AdminOrders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: AdminOrders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Order #{id} status updated to {newStatus}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
