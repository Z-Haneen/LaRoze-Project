using Graduation_Project.Models;
using Graduation_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Graduation_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GraduationDbContext _context;
        private readonly ProductService _productService;

        public HomeController(ILogger<HomeController> logger, GraduationDbContext context, ProductService productService)
        {
            _logger = logger;
            _context = context;
            _productService = productService;
        }

        public IActionResult Index()
        {
            try
            {
                // Create a view model for the home page
                var viewModel = new HomePageViewModel
                {
                    // Get categories
                    Categories = _context.Categories.Take(3).ToList(),

                    // Get featured products (newest products)
                    FeaturedProducts = _context.Products
                        .Include(p => p.Category)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(6)
                        .ToList(),

                    // Get best selling products (for now, just get some products)
                    BestSellingProducts = _context.Products
                        .Include(p => p.Category)
                        .Take(3)
                        .ToList(),

                    // Get active promotions
                    Promotions = _context.Promotions
                        .Where(p => p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
                        .Take(3)
                        .ToList()
                };

                _logger.LogInformation($"Loaded home page data: {viewModel.Categories.Count} categories, " +
                    $"{viewModel.FeaturedProducts.Count} featured products, " +
                    $"{viewModel.BestSellingProducts.Count} best selling products, " +
                    $"{viewModel.Promotions.Count} promotions");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data");
                // Return the view without data in case of error
                return View(new HomePageViewModel());
            }
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

        // Debug action to inspect session state
        public IActionResult DebugSession()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var sessionKeys = HttpContext.Session.Keys;
            var sessionCookie = HttpContext.Request.Cookies["ASP.NET_SessionId"];
            _logger.LogInformation($"DebugSession called. UserId: {userId ?? "null"}, Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}");
            return Content($"UserId: {userId ?? "null"}, Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}");
        }
    }
}
