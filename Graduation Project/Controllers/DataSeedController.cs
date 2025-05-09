//using Graduation_Project.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Graduation_Project.Controllers
//{
//    public class DataSeedController : Controller
//    {
//        private readonly GraduationDbContext _context;
//        private readonly ILogger<DataSeedController> _logger;

//        public DataSeedController(GraduationDbContext context, ILogger<DataSeedController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // GET: /DataSeed/Seed
//        public IActionResult Seed()
//        {
//            try
//            {
//                SeedData.Initialize(_context);
//                return Content("Database seeded successfully!");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while seeding the database.");
//                return Content($"Error seeding database: {ex.Message}");
//            }
//        }

//        // GET: /DataSeed/SeedBasicData
//        public IActionResult SeedBasicData()
//        {
//            try
//            {
//                // Make sure the database is created
//                _context.Database.EnsureCreated();

//                // Seed Roles if they don't exist
//                if (!_context.Roles.Any())
//                {
//                    SeedRoles();
//                }

//                // Seed Users if they don't exist
//                if (!_context.Users.Any())
//                {
//                    SeedUsers();
//                }

//                // Seed Categories if they don't exist
//                if (_context.Categories.Count() <= 3)
//                {
//                    SeedCategories();
//                }

//                // Seed Products if they don't exist
//                if (!_context.Products.Any())
//                {
//                    SeedProducts();
//                }

//                // Seed Promotions if they don't exist
//                if (!_context.Promotions.Any())
//                {
//                    SeedPromotions();
//                }

//                return Content("Basic data seeded successfully!");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while seeding basic data.");
//                return Content($"Error seeding basic data: {ex.Message}");
//            }
//        }

//        private void SeedRoles()
//        {
//            var roles = new List<Role>
//            {
//                new Role { Name = "Admin", Description = "Administrator with full access" },
//                new Role { Name = "Customer", Description = "Regular customer" }
//            };

//            _context.Roles.AddRange(roles);
//            _context.SaveChanges();
//        }

//        private void SeedUsers()
//        {
//            // Get role IDs
//            var adminRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Admin")?.RoleId ?? 1;
//            var customerRoleId = _context.Roles.FirstOrDefault(r => r.Name == "Customer")?.RoleId ?? 2;

//            // Create admin user
//            var adminUser = new User
//            {
//                FirstName = "Admin",
//                LastName = "User",
//                Email = "admin@laroze.com",
//                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
//                Phone = "1234567890",
//                DateOfBirth = new DateTime(1990, 1, 1),
//                Gender = "Male",
//                RegistrationDate = DateTime.Now,
//                LastLogin = DateTime.Now,
//                Status = "Active",
//                RoleId = adminRoleId
//            };

//            // Create customer users
//            var users = new List<User>
//            {
//                adminUser,
//                new User
//                {
//                    FirstName = "John",
//                    LastName = "Doe",
//                    Email = "john@example.com",
//                    Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
//                    Phone = "1234567891",
//                    DateOfBirth = new DateTime(1985, 5, 15),
//                    Gender = "Male",
//                    RegistrationDate = DateTime.Now.AddDays(-30),
//                    LastLogin = DateTime.Now.AddDays(-2),
//                    Status = "Active",
//                    RoleId = customerRoleId
//                }
//            };

//            _context.Users.AddRange(users);
//            _context.SaveChanges();
//        }

//        private void SeedCategories()
//        {
//            // Note: Basic categories (Men, Women, Kids) are already seeded in DbContext
//            // Add subcategories for each main category
//            var categories = new List<Category>
//            {
//                new Category { Name = "Men", Description = "Clothing and accessories for men", ParentCategoryId = null },
//                new Category { Name = "Women", Description = "Clothing and accessories for women", ParentCategoryId = null },
//                new Category { Name = "Kids", Description = "Clothing and accessories for children", ParentCategoryId = null }
//            };

//            _context.Categories.AddRange(categories);
//            _context.SaveChanges();
//        }

//        private void SeedProducts()
//        {
//            // Get category IDs
//            var menCategoryId = _context.Categories.FirstOrDefault(c => c.Name == "Men")?.CategoryId ?? 1;
//            var womenCategoryId = _context.Categories.FirstOrDefault(c => c.Name == "Women")?.CategoryId ?? 2;
//            var kidsCategoryId = _context.Categories.FirstOrDefault(c => c.Name == "Kids")?.CategoryId ?? 3;

//            var products = new List<Product>
//            {
//                // Men's products
//                new Product
//                {
//                    Name = "Classic White T-Shirt",
//                    Description = "A comfortable white t-shirt made from 100% cotton.",
//                    Price = 19.99m,
//                    StockQuantity = 100,
//                    Sku = "MTS001",
//                    CategoryId = menCategoryId,
//                    ImageUrl = "/images/products/men-tshirt-white.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-60),
//                    UpdatedAt = DateTime.Now.AddDays(-60),
//                    Status = "Active"
//                },
//                new Product
//                {
//                    Name = "Blue Denim Jeans",
//                    Description = "Classic blue denim jeans with a straight fit.",
//                    Price = 49.99m,
//                    StockQuantity = 75,
//                    Sku = "MJN001",
//                    CategoryId = menCategoryId,
//                    ImageUrl = "/images/products/men-jeans-blue.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-55),
//                    UpdatedAt = DateTime.Now.AddDays(-55),
//                    Status = "Active"
//                },

//                // Women's products
//                new Product
//                {
//                    Name = "Floral Summer Dress",
//                    Description = "A beautiful floral dress perfect for summer.",
//                    Price = 59.99m,
//                    StockQuantity = 50,
//                    Sku = "WDR001",
//                    CategoryId = womenCategoryId,
//                    ImageUrl = "/images/products/women-dress-floral.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-45),
//                    UpdatedAt = DateTime.Now.AddDays(-45),
//                    Status = "Active"
//                },
//                new Product
//                {
//                    Name = "Black Skinny Jeans",
//                    Description = "Comfortable black skinny jeans for women.",
//                    Price = 44.99m,
//                    StockQuantity = 60,
//                    Sku = "WJN001",
//                    CategoryId = womenCategoryId,
//                    ImageUrl = "/images/products/women-jeans-black.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-40),
//                    UpdatedAt = DateTime.Now.AddDays(-40),
//                    Status = "Active"
//                },

//                // Kids' products
//                new Product
//                {
//                    Name = "Cartoon Print T-Shirt",
//                    Description = "Fun cartoon print t-shirt for kids.",
//                    Price = 14.99m,
//                    StockQuantity = 80,
//                    Sku = "KTS001",
//                    CategoryId = kidsCategoryId,
//                    ImageUrl = "/images/products/kids-tshirt-cartoon.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-30),
//                    UpdatedAt = DateTime.Now.AddDays(-30),
//                    Status = "Active"
//                },
//                new Product
//                {
//                    Name = "Denim Overalls",
//                    Description = "Cute denim overalls for children.",
//                    Price = 29.99m,
//                    StockQuantity = 40,
//                    Sku = "KOV001",
//                    CategoryId = kidsCategoryId,
//                    ImageUrl = "/images/products/kids-overalls-denim.jpg",
//                    CreatedAt = DateTime.Now.AddDays(-25),
//                    UpdatedAt = DateTime.Now.AddDays(-25),
//                    Status = "Active"
//                }
//            };

//            _context.Products.AddRange(products);
//            _context.SaveChanges();
//        }

//        private void SeedPromotions()
//        {
//            var products = _context.Products.ToList();
//            var promotions = new List<Promotion>();
//            var random = new Random();

//            // Add promotions for some products
//            foreach (var product in products.Where(p => p.Price > 30))
//            {
//                // 50% chance to add a promotion
//                if (random.Next(2) == 0)
//                {
//                    promotions.Add(new Promotion
//                    {
//                        ProductId = product.ProductId,
//                        DiscountPercentage = random.Next(10, 31), // 10-30% discount
//                        StartDate = DateTime.Now.AddDays(-10),
//                        EndDate = DateTime.Now.AddDays(20),
//                        PromotionType = "Seasonal Sale"
//                    });
//                }
//            }

//            _context.Promotions.AddRange(promotions);
//            _context.SaveChanges();
//        }
//    }
//}
