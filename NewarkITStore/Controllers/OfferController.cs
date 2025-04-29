using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;
using NewarkITStore.ViewModels;

namespace NewarkITStore.Controllers
{
    public class OfferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OfferController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Offer
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers.Include(o => o.Product).ToListAsync();
            return View(offers);
        }

        // GET: Offer/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        // POST: Offer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Offer offer)
        {
            if (ModelState.IsValid)
            {
                _context.Offers.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = _context.Products.ToList(); // reload products if error
            return View(offer);
        }


        // GET: Offer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
                return NotFound();

            ViewBag.Products = _context.Products.ToList();
            return View(offer);
        }

        // POST: Offer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Offer offer)
        {
            if (id != offer.OfferId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Offers.Any(e => e.OfferId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = _context.Products.ToList();
            return View(offer);
        }
        // GET: Offer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Offers == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .Include(o => o.Product) // if you want to display product name
                .FirstOrDefaultAsync(m => m.OfferId == id);

            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }

        // POST: Offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Offers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Offers'  is null.");
            }

            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                _context.Offers.Remove(offer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
