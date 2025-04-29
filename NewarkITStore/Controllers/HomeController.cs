using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewarkITStore.Models;
using NewarkITStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace NewarkITStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var products = await _context.Products
            .Include(p => p.ProductType)
            .ToListAsync();

        return View(products);
    }


    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.ProductType)
            .ToListAsync();

        var today = DateTime.Today;

        // Load active offers
        var offers = await _context.Offers
            .Where(o => o.StartDate <= today && o.EndDate >= today)
            .ToListAsync();
        ViewBag.Offers = offers;

        // Load user status if logged in
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserStatus = user?.Status;
        }
        else
        {
            ViewBag.UserStatus = null;
        }

        return View(products);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
