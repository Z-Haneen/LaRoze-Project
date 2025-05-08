using Graduation_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Graduation_Project.Data
{
    public static class SeedData
    {
        public static void Initialize(GraduationDbContext context)
        {
            // Make sure the database is created
            context.Database.EnsureCreated();

            // Seed Roles if they don't exist
            if (!context.Roles.Any())
            {
                SeedRoles(context);
            }

            // Seed Users if they don't exist
            if (!context.Users.Any())
            {
                SeedUsers(context);
            }

            // Seed Categories if they don't exist (some are already seeded in DbContext)
            if (context.Categories.Count() <= 3)
            {
                SeedCategories(context);
            }

            // Seed SubCategories if they don't exist
            if (!context.SubCategories.Any())
            {
                SeedSubCategories(context);
            }

            // Seed Products if they don't exist
            if (!context.Products.Any())
            {
                SeedProducts(context);
            }

            // Seed ProductImages if they don't exist
            if (!context.ProductImages.Any())
            {
                SeedProductImages(context);
            }

            // Seed UserAddresses if they don't exist
            if (!context.UserAddresses.Any())
            {
                SeedUserAddresses(context);
            }

            // Seed ProductReviews if they don't exist
            if (!context.ProductReviews.Any())
            {
                SeedProductReviews(context);
            }

            // Seed Promotions if they don't exist
            if (!context.Promotions.Any())
            {
                SeedPromotions(context);
            }

            // Seed Carts and CartItems if they don't exist
            if (!context.Carts.Any())
            {
                SeedCarts(context);
            }

            // Seed Orders, OrderItems, and Payments if they don't exist
            if (!context.Orders.Any())
            {
                SeedOrders(context);
            }

            // Seed Wishlists if they don't exist
            if (!context.Wishlists.Any())
            {
                SeedWishlists(context);
            }

            // Seed Contacts if they don't exist
            if (!context.Contacts.Any())
            {
                SeedContacts(context);
            }
        }

        private static void SeedRoles(GraduationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role { Name = "Admin", Description = "Administrator with full access" },
                new Role { Name = "Customer", Description = "Regular customer" }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        private static void SeedUsers(GraduationDbContext context)
        {
            // Get role IDs
            var adminRoleId = context.Roles.FirstOrDefault(r => r.Name == "Admin")?.RoleId ?? 1;
            var customerRoleId = context.Roles.FirstOrDefault(r => r.Name == "Customer")?.RoleId ?? 2;

            // Create admin user
            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@laroze.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Phone = "1234567890",
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                RegistrationDate = DateTime.Now,
                LastLogin = DateTime.Now,
                Status = "Active",
                RoleId = adminRoleId
            };

            // Create customer users
            var users = new List<User>
            {
                adminUser,
                new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    Phone = "1234567891",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-30),
                    LastLogin = DateTime.Now.AddDays(-2),
                    Status = "Active",
                    RoleId = customerRoleId
                },
                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    Phone = "1234567892",
                    DateOfBirth = new DateTime(1990, 8, 21),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-20),
                    LastLogin = DateTime.Now.AddDays(-1),
                    Status = "Active",
                    RoleId = customerRoleId
                },
                new User
                {
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    Phone = "1234567893",
                    DateOfBirth = new DateTime(1988, 3, 10),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                    LastLogin = DateTime.Now.AddHours(-12),
                    Status = "Active",
                    RoleId = customerRoleId
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        private static void SeedCategories(GraduationDbContext context)
        {
            // Note: Basic categories (Men, Women, Kids) are already seeded in DbContext
            // Add subcategories for each main category
            var categories = new List<Category>
            {
                new Category { Name = "Accessories", Description = "Fashion accessories", ParentCategoryId = 1 },
                new Category { Name = "Footwear", Description = "Shoes and boots", ParentCategoryId = 1 },
                new Category { Name = "Dresses", Description = "Women's dresses", ParentCategoryId = 2 },
                new Category { Name = "Tops", Description = "Women's tops and blouses", ParentCategoryId = 2 },
                new Category { Name = "Boys", Description = "Clothing for boys", ParentCategoryId = 3 },
                new Category { Name = "Girls", Description = "Clothing for girls", ParentCategoryId = 3 }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        private static void SeedSubCategories(GraduationDbContext context)
        {
            var subCategories = new List<SubCategory>();

            // Get category IDs
            var menCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Men")?.CategoryId ?? 1;
            var womenCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Women")?.CategoryId ?? 2;
            var kidsCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Kids")?.CategoryId ?? 3;

            // Add subcategories for Men
            subCategories.AddRange(new List<SubCategory>
            {
                new SubCategory { SubCategoryName = "T-Shirts", CategoryId = menCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Shirts", CategoryId = menCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Pants", CategoryId = menCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Jackets", CategoryId = menCategoryId, IsActive = true }
            });

            // Add subcategories for Women
            subCategories.AddRange(new List<SubCategory>
            {
                new SubCategory { SubCategoryName = "Blouses", CategoryId = womenCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Skirts", CategoryId = womenCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Dresses", CategoryId = womenCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Jeans", CategoryId = womenCategoryId, IsActive = true }
            });

            // Add subcategories for Kids
            subCategories.AddRange(new List<SubCategory>
            {
                new SubCategory { SubCategoryName = "T-Shirts", CategoryId = kidsCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Pants", CategoryId = kidsCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Dresses", CategoryId = kidsCategoryId, IsActive = true },
                new SubCategory { SubCategoryName = "Shoes", CategoryId = kidsCategoryId, IsActive = true }
            });

            context.SubCategories.AddRange(subCategories);
            context.SaveChanges();
        }

        private static void SeedProducts(GraduationDbContext context)
        {
            // Get category IDs
            var menCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Men")?.CategoryId ?? 1;
            var womenCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Women")?.CategoryId ?? 2;
            var kidsCategoryId = context.Categories.FirstOrDefault(c => c.Name == "Kids")?.CategoryId ?? 3;

            var products = new List<Product>
            {
                // Men's products
                new Product
                {
                    Name = "Classic White T-Shirt",
                    Description = "A comfortable white t-shirt made from 100% cotton.",
                    Price = 19.99m,
                    StockQuantity = 100,
                    Sku = "MTS001",
                    CategoryId = menCategoryId,
                    ImageUrl = "/images/products/men-tshirt-white.jpg",
                    CreatedAt = DateTime.Now.AddDays(-60),
                    UpdatedAt = DateTime.Now.AddDays(-60),
                    Status = "Active"
                },
                new Product
                {
                    Name = "Blue Denim Jeans",
                    Description = "Classic blue denim jeans with a straight fit.",
                    Price = 49.99m,
                    StockQuantity = 75,
                    Sku = "MJN001",
                    CategoryId = menCategoryId,
                    ImageUrl = "/images/products/men-jeans-blue.jpg",
                    CreatedAt = DateTime.Now.AddDays(-55),
                    UpdatedAt = DateTime.Now.AddDays(-55),
                    Status = "Active"
                },
                new Product
                {
                    Name = "Black Leather Jacket",
                    Description = "Stylish black leather jacket with a modern fit.",
                    Price = 129.99m,
                    StockQuantity = 30,
                    Sku = "MJK001",
                    CategoryId = menCategoryId,
                    ImageUrl = "/images/products/men-jacket-black.jpg",
                    CreatedAt = DateTime.Now.AddDays(-50),
                    UpdatedAt = DateTime.Now.AddDays(-50),
                    Status = "Active"
                },

                // Women's products
                new Product
                {
                    Name = "Floral Summer Dress",
                    Description = "A beautiful floral dress perfect for summer.",
                    Price = 59.99m,
                    StockQuantity = 50,
                    Sku = "WDR001",
                    CategoryId = womenCategoryId,
                    ImageUrl = "/images/products/women-dress-floral.jpg",
                    CreatedAt = DateTime.Now.AddDays(-45),
                    UpdatedAt = DateTime.Now.AddDays(-45),
                    Status = "Active"
                },
                new Product
                {
                    Name = "Black Skinny Jeans",
                    Description = "Comfortable black skinny jeans for women.",
                    Price = 44.99m,
                    StockQuantity = 60,
                    Sku = "WJN001",
                    CategoryId = womenCategoryId,
                    ImageUrl = "/images/products/women-jeans-black.jpg",
                    CreatedAt = DateTime.Now.AddDays(-40),
                    UpdatedAt = DateTime.Now.AddDays(-40),
                    Status = "Active"
                },
                new Product
                {
                    Name = "White Blouse",
                    Description = "Elegant white blouse with a relaxed fit.",
                    Price = 34.99m,
                    StockQuantity = 45,
                    Sku = "WBL001",
                    CategoryId = womenCategoryId,
                    ImageUrl = "/images/products/women-blouse-white.jpg",
                    CreatedAt = DateTime.Now.AddDays(-35),
                    UpdatedAt = DateTime.Now.AddDays(-35),
                    Status = "Active"
                },

                // Kids' products
                new Product
                {
                    Name = "Cartoon Print T-Shirt",
                    Description = "Fun cartoon print t-shirt for kids.",
                    Price = 14.99m,
                    StockQuantity = 80,
                    Sku = "KTS001",
                    CategoryId = kidsCategoryId,
                    ImageUrl = "/images/products/kids-tshirt-cartoon.jpg",
                    CreatedAt = DateTime.Now.AddDays(-30),
                    UpdatedAt = DateTime.Now.AddDays(-30),
                    Status = "Active"
                },
                new Product
                {
                    Name = "Denim Overalls",
                    Description = "Cute denim overalls for children.",
                    Price = 29.99m,
                    StockQuantity = 40,
                    Sku = "KOV001",
                    CategoryId = kidsCategoryId,
                    ImageUrl = "/images/products/kids-overalls-denim.jpg",
                    CreatedAt = DateTime.Now.AddDays(-25),
                    UpdatedAt = DateTime.Now.AddDays(-25),
                    Status = "Active"
                },
                new Product
                {
                    Name = "Colorful Sneakers",
                    Description = "Comfortable and colorful sneakers for kids.",
                    Price = 24.99m,
                    StockQuantity = 35,
                    Sku = "KSH001",
                    CategoryId = kidsCategoryId,
                    ImageUrl = "/images/products/kids-sneakers-colorful.jpg",
                    CreatedAt = DateTime.Now.AddDays(-20),
                    UpdatedAt = DateTime.Now.AddDays(-20),
                    Status = "Active"
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        private static void SeedProductImages(GraduationDbContext context)
        {
            var products = context.Products.ToList();
            var productImages = new List<ProductImage>();

            foreach (var product in products)
            {
                // Add default image
                productImages.Add(new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = product.ImageUrl,
                    DefaultImage = true
                });

                // Add additional images for each product
                if (product.Name.Contains("T-Shirt"))
                {
                    productImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = product.ImageUrl.Replace(".jpg", "-back.jpg"),
                        DefaultImage = false
                    });
                    productImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = product.ImageUrl.Replace(".jpg", "-detail.jpg"),
                        DefaultImage = false
                    });
                }
                else if (product.Name.Contains("Jeans") || product.Name.Contains("Dress"))
                {
                    productImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = product.ImageUrl.Replace(".jpg", "-side.jpg"),
                        DefaultImage = false
                    });
                    productImages.Add(new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImageUrl = product.ImageUrl.Replace(".jpg", "-back.jpg"),
                        DefaultImage = false
                    });
                }
            }

            context.ProductImages.AddRange(productImages);
            context.SaveChanges();
        }

        private static void SeedUserAddresses(GraduationDbContext context)
        {
            var users = context.Users.Where(u => u.RoleId != 1).ToList(); // Exclude admin
            var addresses = new List<UserAddress>();

            foreach (var user in users)
            {
                // Add a default address for each user
                addresses.Add(new UserAddress
                {
                    UserId = user.UserId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    StreetAddress = "123 Main Street",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10001",
                    Country = "USA",
                    PhoneNumber = user.Phone,
                    IsDefault = true
                });

                // Add a secondary address for some users
                if (user.UserId % 2 == 0)
                {
                    addresses.Add(new UserAddress
                    {
                        UserId = user.UserId,
                        FullName = $"{user.FirstName} {user.LastName}",
                        StreetAddress = "456 Oak Avenue",
                        ApartmentNumber = "Apt 7B",
                        City = "Los Angeles",
                        State = "CA",
                        PostalCode = "90001",
                        Country = "USA",
                        PhoneNumber = user.Phone,
                        IsDefault = false
                    });
                }
            }

            context.UserAddresses.AddRange(addresses);
            context.SaveChanges();
        }

        private static void SeedProductReviews(GraduationDbContext context)
        {
            var users = context.Users.Where(u => u.RoleId != 1).ToList(); // Exclude admin
            var products = context.Products.ToList();
            var reviews = new List<ProductReview>();
            var random = new Random();

            // Add reviews for each product
            foreach (var product in products)
            {
                // Add 1-3 reviews per product
                int reviewCount = random.Next(1, 4);
                for (int i = 0; i < reviewCount; i++)
                {
                    // Get a random user
                    var user = users[random.Next(users.Count)];

                    // Create a review
                    reviews.Add(new ProductReview
                    {
                        UserId = user.UserId,
                        ProductId = product.ProductId,
                        Rating = random.Next(3, 6), // 3-5 star ratings
                        ReviewText = GetRandomReview(product.Name),
                        CreatedAt = DateTime.Now.AddDays(-random.Next(1, 30))
                    });
                }
            }

            context.ProductReviews.AddRange(reviews);
            context.SaveChanges();
        }

        private static string GetRandomReview(string productName)
        {
            var reviews = new List<string>
            {
                $"I love this {productName}! The quality is excellent and it fits perfectly.",
                $"Great product! The {productName} exceeded my expectations.",
                $"The {productName} is good, but I expected better quality for the price.",
                $"Very comfortable and stylish {productName}. Would definitely recommend!",
                $"This {productName} is exactly what I was looking for. Fast shipping too!",
                $"Nice {productName}, but the color is slightly different from the pictures.",
                $"Excellent quality and value for money. This {productName} is a must-have!",
                $"The {productName} is perfect for everyday use. Very satisfied with my purchase."
            };

            return reviews[new Random().Next(reviews.Count)];
        }

        private static void SeedPromotions(GraduationDbContext context)
        {
            var products = context.Products.ToList();
            var promotions = new List<Promotion>();
            var random = new Random();

            // Add promotions for some products
            foreach (var product in products.Where(p => p.Price > 30))
            {
                // 50% chance to add a promotion
                if (random.Next(2) == 0)
                {
                    promotions.Add(new Promotion
                    {
                        ProductId = product.ProductId,
                        DiscountPercentage = random.Next(10, 31), // 10-30% discount
                        StartDate = DateTime.Now.AddDays(-10),
                        EndDate = DateTime.Now.AddDays(20),
                        PromotionType = "Seasonal Sale"
                    });
                }
            }

            context.Promotions.AddRange(promotions);
            context.SaveChanges();
        }

        private static void SeedCarts(GraduationDbContext context)
        {
            var users = context.Users.Where(u => u.RoleId != 1).ToList(); // Exclude admin
            var products = context.Products.ToList();
            var carts = new List<Cart>();
            var cartItems = new List<CartItem>();
            var random = new Random();

            // Create a cart for each user
            foreach (var user in users)
            {
                var cart = new Cart
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 10))
                };

                carts.Add(cart);
            }

            context.Carts.AddRange(carts);
            context.SaveChanges();

            // Add items to each cart
            foreach (var cart in carts)
            {
                // Add 1-3 items to each cart
                int itemCount = random.Next(1, 4);
                for (int i = 0; i < itemCount; i++)
                {
                    // Get a random product
                    var product = products[random.Next(products.Count)];

                    // Create a cart item
                    cartItems.Add(new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = product.ProductId,
                        Quantity = random.Next(1, 4)
                    });
                }
            }

            context.CartItems.AddRange(cartItems);
            context.SaveChanges();
        }

        private static void SeedOrders(GraduationDbContext context)
        {
            try
            {
                var users = context.Users.Where(u => u.RoleId != 1).ToList(); // Exclude admin
                var products = context.Products.ToList();
                var addresses = context.UserAddresses.ToList();
                var orders = new List<Order>();
                var orderItems = new List<OrderItem>();
                var random = new Random();

                // Create 1-2 orders for each user
                foreach (var user in users)
                {
                    int orderCount = random.Next(1, 3);
                    for (int i = 0; i < orderCount; i++)
                    {
                        // Get a random address for this user
                        var address = addresses.FirstOrDefault(a => a.UserId == user.UserId);
                        if (address == null) continue;

                        // Create an order first (without PaymentId)
                        var order = new Order
                        {
                            UserId = user.UserId,
                            ShippingAddressId = address.AddressId, // This maps to the AddressId property in UserAddress
                            OrderDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                            Status = GetRandomOrderStatus(),
                            TrackingNumber = GenerateTrackingNumber(),
                            TotalPrice = 0 // Will be updated after adding order items
                        };

                        // Add 1-4 items to the order
                        int itemCount = random.Next(1, 5);
                        decimal totalPrice = 0;

                        var orderItemsList = new List<OrderItem>();
                        for (int j = 0; j < itemCount; j++)
                        {
                            // Get a random product
                            var product = products[random.Next(products.Count)];
                            int quantity = random.Next(1, 4);
                            decimal price = product.Price;

                            // Create an order item
                            var orderItem = new OrderItem
                            {
                                ProductId = product.ProductId,
                                Quantity = quantity,
                                Price = price,
                                ProductName = product.Name,
                                ProductSku = product.Sku,
                                ProductImage = product.ImageUrl
                            };

                            orderItemsList.Add(orderItem);
                            totalPrice += price * quantity;
                        }

                        // Update order with total price
                        order.TotalPrice = totalPrice;

                        // Save the order first
                        context.Orders.Add(order);
                        context.SaveChanges();

                        // Now create a payment with the OrderId
                        var payment = new Payment
                        {
                            OrderId = order.OrderId,
                            PaymentMethod = random.Next(2) == 0 ? "Credit Card" : "Cash on Delivery",
                            PaymentStatus = "Completed",
                            TransactionId = Guid.NewGuid().ToString().Substring(0, 8),
                            PaymentDate = order.OrderDate,
                            Amount = totalPrice,
                            PaymentDetails = "Payment processed successfully"
                        };

                        // Save the payment
                        context.Payments.Add(payment);
                        context.SaveChanges();

                        // Update the order with the PaymentId
                        order.PaymentId = payment.PaymentId;
                        context.SaveChanges();

                        // Now add the order items
                        foreach (var item in orderItemsList)
                        {
                            item.OrderId = order.OrderId;
                            context.OrderItems.Add(item);
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedOrders: {ex.Message}");
                // Continue with other seeding operations
            }
        }

        private static string GetRandomOrderStatus()
        {
            // These statuses match the ones defined in the Order.GetStatusId method
            var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered" };
            return statuses[new Random().Next(statuses.Length)];
        }

        private static string GenerateTrackingNumber()
        {
            return $"TRK{DateTime.Now.ToString("yyyyMMdd")}{new Random().Next(1000, 9999)}";
        }

        private static void SeedWishlists(GraduationDbContext context)
        {
            var users = context.Users.Where(u => u.RoleId != 1).ToList(); // Exclude admin
            var products = context.Products.ToList();
            var wishlists = new List<Wishlist>();
            var random = new Random();

            // Add 2-5 products to each user's wishlist
            foreach (var user in users)
            {
                int wishlistCount = random.Next(2, 6);
                var userProducts = products.OrderBy(x => random.Next()).Take(wishlistCount).ToList();

                foreach (var product in userProducts)
                {
                    wishlists.Add(new Wishlist
                    {
                        UserId = user.UserId,
                        ProductId = product.ProductId
                    });
                }
            }

            context.Wishlists.AddRange(wishlists);
            context.SaveChanges();
        }

        private static void SeedContacts(GraduationDbContext context)
        {
            var users = context.Users.ToList();
            var contacts = new List<Contact>();
            var random = new Random();

            // Create 1-2 contact messages for some users
            foreach (var user in users.Where(u => random.Next(3) == 0)) // 1/3 chance
            {
                contacts.Add(new Contact
                {
                    UserId = user.UserId,
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Subject = GetRandomContactSubject(),
                    Message = GetRandomContactMessage()
                });

                // Add a second message for some users
                if (random.Next(2) == 0) // 50% chance
                {
                    contacts.Add(new Contact
                    {
                        UserId = user.UserId,
                        Name = $"{user.FirstName} {user.LastName}",
                        Email = user.Email,
                        Subject = GetRandomContactSubject(),
                        Message = GetRandomContactMessage()
                    });
                }
            }

            context.Contacts.AddRange(contacts);
            context.SaveChanges();
        }

        private static string GetRandomContactSubject()
        {
            var subjects = new[]
            {
                "Question about my order",
                "Product availability inquiry",
                "Return policy question",
                "Shipping information",
                "Account assistance needed",
                "Product recommendation request",
                "Website feedback"
            };

            return subjects[new Random().Next(subjects.Length)];
        }

        private static string GetRandomContactMessage()
        {
            var messages = new[]
            {
                "I recently placed an order and I'm wondering when it will be shipped. Can you provide an update?",
                "Do you have this product in a different size/color? I couldn't find it on the website.",
                "I need to return an item. What is your return policy and process?",
                "How long does shipping typically take to my location?",
                "I'm having trouble accessing my account. Can you help me reset my password?",
                "Can you recommend products similar to the one I recently purchased?",
                "I love your website design, but I think the checkout process could be improved."
            };

            return messages[new Random().Next(messages.Length)];
        }
    }
}
