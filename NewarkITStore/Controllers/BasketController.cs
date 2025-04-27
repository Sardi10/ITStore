
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;
using NewarkITStore.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace NewarkITStore.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BasketController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
        public async Task<IActionResult> Checkout(string CardholderName, string CardNumber, string Expiration, string CVV, int ShippingAddressId)
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

            int? selectedCardId = null;
            string savedCardIdRaw = Request.Form["SavedCardId"];

            if (!string.IsNullOrEmpty(savedCardIdRaw) && int.TryParse(savedCardIdRaw, out var parsedId) && parsedId > 0)
            {
                // Use selected saved card
                selectedCardId = parsedId;
            }
            else
            {
                // Create a new CreditCard record
                var newCard = new CreditCard
                {
                    UserId = userId,
                    CardNumber = Request.Form["CardNumber"],
                    SecurityCode = Request.Form["CVV"],
                    CardHolderName = Request.Form["CardholderName"],
                    ExpiryDate = DateTime.TryParse(Request.Form["Expiration"], out var expiry) ? expiry : DateTime.UtcNow.AddYears(2),
                    CardType = Request.Form["CardType"], // Optional: if you're using dropdown for Visa/Amex/etc.
                    BillingStreet = Request.Form["BillingStreet"],
                    BillingCity = Request.Form["BillingCity"],
                    BillingState = Request.Form["BillingState"],
                    BillingCountry = Request.Form["BillingCountry"],
                    BillingZip = Request.Form["BillingZip"]
                };

                _context.CreditCards.Add(newCard);
                await _context.SaveChangesAsync(); // Needed to generate the ID
                selectedCardId = newCard.CreditCardId;
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                CreditCardId = selectedCardId,
                ShippingAddressId = int.TryParse(Request.Form["ShippingAddressId"], out var addrId) ? addrId : null,
                TotalAmount = basketItems.Sum(i => i.PricePerUnit * i.Quantity * 1.1m), // including 10% tax
                OrderItems = basketItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PricePerUnit = item.PricePerUnit
                }).ToList()
            };


            _context.Orders.Add(order);
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
        public async Task<IActionResult> OrderHistory(string searchCustomer, string searchProduct, DateTime? startDate, DateTime? endDate)
        {
            var userId = _userManager.GetUserId(User);

            var ordersQuery = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .Include(o => o.CreditCard)
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchProduct))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderItems.Any(oi => oi.Product.Name.Contains(searchProduct)));
            }

            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            var orders = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }


        [HttpGet]
        public async Task<IActionResult> CheckoutReview()
        {
            var userId = _userManager.GetUserId(User);

            var basketItems = await _context.BasketItems
                .Include(b => b.Product)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var addresses = await _context.ShippingAddresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var cards = await _context.CreditCards
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!basketItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            return View(new CheckoutViewModel
            {
                BasketItems = basketItems,
                ShippingAddresses = addresses,
                SavedCards = cards
            });
        }


    }
}
