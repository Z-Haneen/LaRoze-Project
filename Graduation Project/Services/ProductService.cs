using Graduation_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Graduation_Project.Services
{
    public class ProductService
    {
        private readonly GraduationDbContext _context;

        public ProductService(GraduationDbContext context)
        {
            _context = context;
        }

        // دالة لاسترجاع جميع التصنيفات
        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        // دالة لاسترجاع جميع المنتجات
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // دالة لاسترجاع منتج حسب الـ ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // دالة لإضافة منتج جديد
        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        // دالة لتحديث منتج
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // دالة لحذف منتج
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