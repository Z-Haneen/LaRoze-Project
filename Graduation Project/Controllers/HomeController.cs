using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Services;
using System.Diagnostics;
using Graduation_Project.Models;

namespace Graduation_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;

        public HomeController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Shop()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        public IActionResult Privacy()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.Categories = _productService.GetCategories();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}