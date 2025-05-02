using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Services;
using System.Diagnostics;
using Graduation_Project.Models;
using Microsoft.Extensions.Logging;

namespace Graduation_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ProductService productService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"Index called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Shop()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"Shop called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult About()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"About called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Contact()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"Contact called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Privacy()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"Privacy called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"Error called. Session UserId: {userIdStr ?? "null"}");
            ViewBag.Categories = _productService.GetCategories();
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