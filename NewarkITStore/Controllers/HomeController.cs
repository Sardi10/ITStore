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
        var products = await _context.Products.Include(p => p.ProductType).ToListAsync();
        var offers = await _context.Offers.ToListAsync();

        ViewBag.Offers = offers;
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserStatus = user?.Status ?? "Regular";
        }
        else
        {
            ViewBag.UserStatus = "Regular";
        }

        return View(products);
    }

    public async Task<IActionResult> SearchProducts(string term)
    {
        var query = _context.Products.Include(p => p.ProductType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(p => p.Name.Contains(term) || p.ProductType.Name.Contains(term));
        }

        var products = await query.ToListAsync();
        var offers = await _context.Offers.ToListAsync();

        ViewBag.Offers = offers;
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.UserStatus = user?.Status ?? "Regular";
        }
        else
        {
            ViewBag.UserStatus = "Regular";
        }

        return PartialView("_ProductCards", products);
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
