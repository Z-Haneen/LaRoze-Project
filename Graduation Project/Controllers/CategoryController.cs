//by fady
using Graduation_Project.Models;
using Graduation_Project.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Graduation_Project.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ProductService productService, ILogger<CategoryController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: Category/Products/1
        public async Task<IActionResult> Products(int categoryId)
        {
            _logger.LogInformation($"Fetching products for CategoryId: {categoryId}");

            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            var category = await _productService.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                _logger.LogWarning($"Category with ID {categoryId} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Found {products.Count} products for CategoryId: {categoryId}");

            ViewData["CategoryName"] = category.Name;
            ViewData["CategoryId"] = categoryId;

            return View(products); // Removed explicit path to let ASP.NET Core resolve the view
        }
        // GET: Category/ProductDetails/1
        public async Task<IActionResult> ProductDetails(int productId)
        {
            _logger.LogInformation($"Fetching details for ProductId: {productId}");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found.");
                return NotFound();
            }

            _logger.LogInformation($"Found product: {product.Name} for ProductId: {productId}");
            return View(product);
        }
    }
}