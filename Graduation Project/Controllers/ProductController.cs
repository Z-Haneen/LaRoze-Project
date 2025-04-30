using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Graduation_Project.Services;

namespace Graduation_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            var categories = _productService.GetCategories();
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "CategoryId", "Name");
            return View(new ProductDto());
        }

        // POST: Product/Create
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = _productService.GetCategories();
                ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "CategoryId", "Name");
                return View(dto);
            }

            string imageUrl = "/images/products/no-image.png"; // Default image

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid() + "_" + dto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                imageUrl = "/images/products/" + uniqueFileName;
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Sku = dto.Sku,
                CategoryId = dto.CategoryId,
                Status = dto.Status,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var dto = new ProductDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Sku = product.Sku,
                CategoryId = product.CategoryId,
                Status = product.Status
            };

            var categories = _productService.GetCategories();
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "CategoryId", "Name", product.CategoryId);
            return View(dto);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var categories = _productService.GetCategories();
                ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(categories, "CategoryId", "Name", dto.CategoryId);
                return View(dto);
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            string imageUrl = product.ImageUrl; // Keep existing image unless changed

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Delete old image if exists
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Upload new image
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid() + "_" + dto.ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                imageUrl = "/images/products/" + uniqueFileName;
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.Sku = dto.Sku;
            product.CategoryId = dto.CategoryId;
            product.Status = dto.Status;
            product.ImageUrl = imageUrl;
            product.UpdatedAt = DateTime.Now;

            await _productService.UpdateProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            // Delete image file
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            // Delete from database
            await _productService.DeleteProductAsync(id);

            // Set success message
            TempData["SuccessMessage"] = $"Product '{product.Name}' has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}