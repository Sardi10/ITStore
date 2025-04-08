using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;

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

        public async Task<IActionResult> Add(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var existingItem = await _context.BasketItems
                .FirstOrDefaultAsync(b => b.ProductId == productId && b.UserId == userId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return NotFound();

                _context.BasketItems.Add(new BasketItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    PricePerUnit = product.RecommendedPrice
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.BasketItems.FindAsync(id);
            if (item != null)
            {
                _context.BasketItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
