using Graduation_Project.Models;
using Graduation_Project.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Graduation_Project.Controllers
{
    [AdminAuthorization]
    public class AdminController : Controller
    {
        private readonly GraduationDbContext _context;

        public AdminController(GraduationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Dashboard stats
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.OrderCount = await _context.Orders.CountAsync();
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();

            // Calculate total revenue
            ViewBag.TotalRevenue = await _context.Orders
                .Where(o => o.StatusId == 3) // 3 is the StatusId for "Delivered" which we'll consider as "Completed"
                .SumAsync(o => o.TotalPrice);

            // Recent orders
            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            // Recent products
            ViewBag.RecentProducts = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Recent users
            ViewBag.RecentUsers = await _context.Users
                .OrderByDescending(u => u.RegistrationDate)
                .Take(5)
                .ToListAsync();

            // Get contact messages if they exist
            ViewBag.RecentMessages = await _context.Contacts
                .OrderByDescending(c => c.ContactId) // Using ID as proxy for creation date
                .Take(3)
                .ToListAsync();

            // Monthly sales data for chart (last 6 months)
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlySales = await _context.Orders
                .Where(o => o.OrderDate >= sixMonthsAgo)
                .GroupBy(o => new { Month = o.OrderDate.Month, Year = o.OrderDate.Year })
                .Select(g => new {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Total = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // Format the data for the chart
            var labels = new List<string>();
            var data = new List<decimal>();

            // Get all months in the range
            for (int i = 0; i < 6; i++)
            {
                var date = DateTime.Now.AddMonths(-5 + i);
                var monthName = date.ToString("MMM yyyy");
                labels.Add(monthName);

                var monthlySale = monthlySales.FirstOrDefault(m => m.Month == date.Month && m.Year == date.Year);
                data.Add(monthlySale?.Total ?? 0);
            }

            ViewBag.ChartLabels = labels;
            ViewBag.ChartData = data;

            return View(recentOrders);
        }



        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.ParentCategory)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // GET: Admin/CreateCategory
        public async Task<IActionResult> CreateCategory()
        {
            // Get all categories for parent selection
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Admin/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // Get all categories for parent selection in case we need to return to the view
                ViewBag.Categories = await _context.Categories.ToListAsync();
                
                // Validate that we're not creating a circular reference
                if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value > 0)
                {
                    // Check if the parent exists
                    var parent = await _context.Categories.FindAsync(category.ParentCategoryId.Value);
                    if (parent == null)
                    {
                        ModelState.AddModelError("ParentCategoryId", "Selected parent category does not exist.");
                        return View(category);
                    }
                }
                
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Categories));
            }

            // If we got this far, something failed, redisplay form
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(category);
        }

        // GET: Admin/EditCategory/5
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Get all categories for parent selection, excluding the current one
            ViewBag.Categories = await _context.Categories
                .Where(c => c.CategoryId != id)
                .ToListAsync();

            return View(category);
        }

        // POST: Admin/EditCategory/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Get all categories for parent selection, excluding the current one
                ViewBag.Categories = await _context.Categories
                    .Where(c => c.CategoryId != id)
                    .ToListAsync();
                
                // Check for circular reference
                if (category.ParentCategoryId.HasValue)
                {
                    // Self-reference check
                    if (category.ParentCategoryId.Value == category.CategoryId)
                    {
                        ModelState.AddModelError("ParentCategoryId", "A category cannot be its own parent.");
                        return View(category);
                    }
                    
                    // Check for circular reference in the hierarchy
                    if (await HasCircularReference(category.CategoryId, category.ParentCategoryId.Value))
                    {
                        ModelState.AddModelError("ParentCategoryId", "Circular reference detected in category hierarchy.");
                        return View(category);
                    }
                }
                
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Categories));
            }

            // If we get here, something failed, redisplay form
            ViewBag.Categories = await _context.Categories
                .Where(c => c.CategoryId != id)
                .ToListAsync();
            return View(category);
        }

        // Helper method to check for circular references in category hierarchy
        private async Task<bool> HasCircularReference(int categoryId, int parentId)
        {
            // If parent is the category itself, it's a circular reference
            if (categoryId == parentId)
                return true;
                
            // Get the parent category
            var parent = await _context.Categories.FindAsync(parentId);
            if (parent == null)
                return false;
                
            // If parent has no parent, we're good
            if (!parent.ParentCategoryId.HasValue)
                return false;
                
            // Recursively check parent's parent to see if we eventually reach back to our category
            return await HasCircularReference(categoryId, parent.ParentCategoryId.Value);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        // GET: Admin/DeleteCategory/5
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products or child categories
            ViewBag.HasProducts = category.Products.Any();
            ViewBag.HasChildCategories = category.ChildCategories.Any();

            return View(category);
        }

        // POST: Admin/DeleteCategory/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products or child categories
            if (category.Products.Any() || category.ChildCategories.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete category because it has products or child categories associated with it.";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Categories));
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Admin/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Update status
            string oldStatus = order.Status;
            order.Status = status;

            // Update date fields based on status
            if (status == "Shipped" && oldStatus != "Shipped")
            {
                order.ShippedDate = DateTime.Now;
            }
            else if (status == "Delivered" && oldStatus != "Delivered")
            {
                order.DeliveredDate = DateTime.Now;
            }

            // Update payment status if it exists
            if (order.PaymentId.HasValue)
            {
                var payment = await _context.Payments.FindAsync(order.PaymentId.Value);
                if (payment != null)
                {
                    if (status == "Delivered" || status == "Completed")
                    {
                        payment.PaymentStatus = "Completed";
                    }
                    else if (status == "Cancelled")
                    {
                        payment.PaymentStatus = "Cancelled";
                    }
                    _context.Update(payment);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating order status: " + ex.Message;
            }

            return RedirectToAction(nameof(OrderDetails), new { id = orderId });
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/UserDetails/5
        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // Get user's orders
            var orders = await _context.Orders
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            ViewBag.Orders = orders;

            // Get all roles for the dropdown
            ViewBag.Roles = await _context.Roles.ToListAsync();

            return View(user);
        }

        // POST: Admin/UpdateUserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(int userId, int roleId, string status)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Update role and status
                user.RoleId = roleId;
                user.Status = status;

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating user: " + ex.Message;
            }

            return RedirectToAction(nameof(UserDetails), new { id = userId });
        }

        // GET: Admin/CreateUser
        public async Task<IActionResult> CreateUser()
        {
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View();
        }

        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email already exists
                    if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        ViewBag.Roles = await _context.Roles.ToListAsync();
                        return View(user);
                    }

                    // Hash the password
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    // Set default values
                    user.RegistrationDate = DateTime.Now;
                    user.LastLogin = DateTime.Now;
                    user.Status = "Active";

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(Users));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating user: " + ex.Message);
                    TempData["ErrorMessage"] = "Failed to create user. Please try again.";
                }
            }

            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(user);
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            // Check if user has orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
            ViewBag.HasOrders = hasOrders;

            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                // Check if this is the only admin user
                if (user.RoleId == 1) // Admin role
                {
                    var adminCount = await _context.Users.CountAsync(u => u.RoleId == 1);
                    if (adminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Cannot delete the only admin user.";
                        return RedirectToAction(nameof(Users));
                    }
                }

                // Delete user
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting user: " + ex.Message;
            }

            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Products
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(products);
        }

        // GET: Admin/CreateProduct
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile mainImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload
                    if (mainImage != null && mainImage.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(mainImage.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("mainImage", "Invalid file type. Only JPG, PNG, and GIF images are allowed.");
                            ViewBag.Categories = await _context.Categories.ToListAsync();
                            return View(product);
                        }

                        // Create directory if it doesn't exist
                        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }

                        product.ImageUrl = "/images/products/" + fileName;
                    }
                    else
                    {
                        // Set default image if no image is uploaded
                        product.ImageUrl = "/images/products/default.jpg";
                    }

                    product.CreatedAt = DateTime.Now;
                    product.UpdatedAt = DateTime.Now;
                    product.Status = "Active"; // Set default status

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product created successfully!";
                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating product: " + ex.Message);
                    TempData["ErrorMessage"] = "Failed to create product. Please try again.";
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // GET: Admin/EditProduct/5
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile mainImage)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing product to preserve data not in the form
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Preserve the original image URL if no new image is uploaded
                    if (mainImage == null || mainImage.Length == 0)
                    {
                        product.ImageUrl = existingProduct.ImageUrl;
                    }
                    else
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(mainImage.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("mainImage", "Invalid file type. Only JPG, PNG, and GIF images are allowed.");
                            ViewBag.Categories = await _context.Categories.ToListAsync();
                            return View(product);
                        }

                        // Create directory if it doesn't exist
                        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Guid.NewGuid().ToString() + extension;
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await mainImage.CopyToAsync(stream);
                        }

                        // Delete old image if exists and it's not the default image
                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl) &&
                            existingProduct.ImageUrl != "/images/products/default.jpg")
                        {
                            try
                            {
                                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                    existingProduct.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log the error but continue with the update
                                Console.WriteLine($"Error deleting old image: {ex.Message}");
                            }
                        }

                        product.ImageUrl = "/images/products/" + fileName;
                    }

                    // Preserve creation date
                    product.CreatedAt = existingProduct.CreatedAt;
                    product.UpdatedAt = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction(nameof(Products));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating product: " + ex.Message);
                    TempData["ErrorMessage"] = "Failed to update product. Please try again.";
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // GET: Admin/DeleteProduct/5
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            try
            {
                // Delete product image if it exists and it's not the default image
                if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "/images/products/default.jpg")
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting product: " + ex.Message;
            }

            return RedirectToAction(nameof(Products));
        }
    }
}









