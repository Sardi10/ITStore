using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;

namespace NewarkITStore.Controllers
{
    [Authorize]
    public class ShippingAddressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShippingAddressController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ShippingAddress
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var addresses = await _context.ShippingAddresses
                .Where(a => a.UserId == user.Id)
                .ToListAsync();
            return View(addresses);
        }

        // GET: ShippingAddress/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShippingAddress/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShippingAddress address)
        {
            var user = await _userManager.GetUserAsync(User);
            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                address.UserId = user.Id;
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(address);
        }

        // GET: ShippingAddress/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var address = await _context.ShippingAddresses.FindAsync(id);
            if (address == null || address.UserId != _userManager.GetUserId(User))
                return Unauthorized();

            return View(address);
        }

        // POST: ShippingAddress/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ShippingAddress address)
        {
            if (id != address.Id)
                return NotFound();

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    address.UserId = user.Id;

                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ShippingAddresses.Any(e => e.Id == address.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(address);
        }

        // GET: ShippingAddress/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var address = await _context.ShippingAddresses
                .FirstOrDefaultAsync(m => m.Id == id);

            if (address == null || address.UserId != _userManager.GetUserId(User))
                return Unauthorized();

            return View(address);
        }

        // POST: ShippingAddress/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.ShippingAddresses.FindAsync(id);
            if (address == null || address.UserId != _userManager.GetUserId(User))
                return Unauthorized();

            _context.ShippingAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
