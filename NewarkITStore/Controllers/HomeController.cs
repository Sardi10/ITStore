using System.Diagnostics;
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

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
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
