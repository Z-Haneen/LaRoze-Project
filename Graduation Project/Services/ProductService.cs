using Graduation_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Graduation_Project.Services
{
    public class ProductService
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<ProductService> _logger;


        public ProductService(GraduationDbContext context)
        {
            _context = context;
        }


        public ProductService(GraduationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation($"Querying products for CategoryId: {categoryId}");

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            if (!products.Any())
            {
                _logger.LogWarning($"No products found for CategoryId {categoryId}.");
            }
            else
            {
                _logger.LogInformation($"Found {products.Count} products for CategoryId: {categoryId}: {string.Join(", ", products.Select(p => $"ProductId: {p.ProductId}, Name: {p.Name}"))}");
            }

            return products;
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}