
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NewarkITStore.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BasketController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _context.BasketItems
                                      .Include(b => b.Product)
                                      .Where(b => b.UserId == userId)
                                      .ToListAsync();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return NotFound();

            var basketItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.ProductId == productId && b.UserId == userId);

            if (basketItem != null)
            {
                basketItem.Quantity++;
            }
            else
            {
                basketItem = new BasketItem
                {
                    ProductId = productId,
                    UserId = userId,
                    Quantity = 1,
                    PricePerUnit = product.RecommendedPrice
                };
                _context.BasketItems.Add(basketItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int id)
        {
            var item = await _context.BasketItems.Include(b => b.Product).FirstOrDefaultAsync(b => b.BasketItemId == id);
            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketRow", item);
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int id)
        {
            var item = await _context.BasketItems.Include(b => b.Product).FirstOrDefaultAsync(b => b.BasketItemId == id);
            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                await _context.SaveChangesAsync();
            }

            return PartialView("_BasketRow", item);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int basketItemId)
        {
            var item = await _context.BasketItems.FindAsync(basketItemId);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult CartSummary()
        {
            var userId = _userManager.GetUserId(User);
            var basket = _context.BasketItems
                                 .Include(b => b.Product)
                                 .Where(b => b.UserId == userId)
                                 .ToList();

            return PartialView("_BasketSummary", basket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var basketItems = await _context.BasketItems
                .Include(b => b.Product)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            if (!basketItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Create the order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = basketItems.Sum(i => i.PricePerUnit * i.Quantity),
                OrderItems = basketItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PricePerUnit = item.PricePerUnit
                }).ToList()
            };

            _context.Orders.Add(order);

            // Clear basket
            _context.BasketItems.RemoveRange(basketItems);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var userId = _userManager.GetUserId(User);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

    }
}
