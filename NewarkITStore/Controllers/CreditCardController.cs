using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewarkITStore.Data;
using NewarkITStore.Models;
using Microsoft.AspNetCore.Identity;

namespace NewarkITStore.Controllers
{
    [Authorize]
    public class CreditCardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreditCardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CreditCard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cards = await _context.CreditCards
                                      .Where(c => c.UserId == user.Id)
                                      .ToListAsync();
            return View(cards);
        }

        // GET: CreditCard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CreditCard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreditCard creditCard)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // Assign the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge(); // or redirect to login
                }

                creditCard.UserId = user.Id;
                creditCard.User = user;

                _context.Add(creditCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(creditCard);
        }

        // GET: CreditCard/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _context.CreditCards.FindAsync(id);
            if (card == null || card.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(card);
        }

        // POST: CreditCard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreditCard card)
        {
            if (id != card.CreditCardId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    card.UserId = user.Id;

                    _context.Update(card);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CreditCards.Any(e => e.CreditCardId == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(card);
        }

        // GET: CreditCard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(c => c.CreditCardId == id);

            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // GET: CreditCard/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _context.CreditCards
                .FirstOrDefaultAsync(m => m.CreditCardId == id && m.UserId == _userManager.GetUserId(User));

            if (card == null)
                return NotFound();

            return View(card);
        }

        // POST: CreditCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var card = await _context.CreditCards.FindAsync(id);
            _context.CreditCards.Remove(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
