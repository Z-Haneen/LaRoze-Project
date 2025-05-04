using Graduation_Project.Models;
using Graduation_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Graduation_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(GraduationDbContext context, ProductService productService, ILogger<CartController> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        // GET: Cart/Index
        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) && TempData["UserId"] != null)
            {
                userIdStr = TempData["UserId"].ToString();
                HttpContext.Session.SetString("UserId", userIdStr);
                await HttpContext.Session.CommitAsync();
                TempData.Remove("UserId");
            }

            var sessionKeys = HttpContext.Session.Keys;
            var sessionCookie = HttpContext.Request.Cookies["ASP.NET_SessionId"];
            _logger.LogInformation($"Cart/Index called. Session UserId: {userIdStr ?? "null"}, Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session. Falling back to TempData failed.");
                TempData["ErrorMessage"] = "Please log in to view your cart.";
                return RedirectToAction("Index", "Login");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                _logger.LogInformation($"No cart found for UserId {userId}. Creating new cart.");
                cart = new Cart { UserId = userId, CreatedAt = DateTime.Now, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation($"Cart retrieved for UserId {userId} with {cart.CartItems.Count} items.");
            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userIdStr = HttpContext.Session.GetString("UserId") ?? TempData["UserId"]?.ToString();
            var sessionKeys = HttpContext.Session.Keys;
            var sessionCookie = HttpContext.Request.Cookies["ASP.NET_SessionId"];
            _logger.LogInformation($"AddToCart called. Session UserId: {userIdStr ?? "null"}, TempData UserId: {TempData["UserId"]?.ToString() ?? "null"}, Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}, ProductId: {productId}, Quantity: {quantity}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session. TempData fallback also failed.");
                return Json(new { success = false, message = "Please log in to add items to your cart." });
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {productId} not found.");
                return Json(new { success = false, message = "Product not found." });
            }

            if (quantity > product.StockQuantity)
            {
                _logger.LogWarning($"Cannot add requested quantity for ProductId {productId}. Requested: {quantity}, Available: {product.StockQuantity}");
                return Json(new { success = false, message = $"Out of Stock. Only {product.StockQuantity} items available." });
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                _logger.LogInformation($"No cart found for UserId {userId}. Creating new cart.");
                cart = new Cart { UserId = userId, CreatedAt = DateTime.Now, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                var newQuantity = cartItem.Quantity + quantity;
                var availableQuantity = product.StockQuantity + cartItem.Quantity;
                if (newQuantity > availableQuantity)
                {
                    _logger.LogWarning($"Cannot add more items than available stock for ProductId {productId}. Current: {cartItem.Quantity}, Requested: {quantity}, Available: {availableQuantity}");
                    return Json(new { success = false, message = $"Out of Stock. Only {availableQuantity} items available." });
                }
                cartItem.Quantity = newQuantity;
            }
            else
            {
                cartItem = new CartItem { CartId = cart.CartId, ProductId = productId, Quantity = quantity };
                cart.CartItems.Add(cartItem);
            }

            // Reduce stock quantity
            product.StockQuantity -= quantity;
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Added {quantity} of ProductId {productId} to cart for UserId {userId}. Stock updated to {product.StockQuantity}");

            return Json(new { success = true, message = $"{product.Name} added to cart!" });
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId") ?? TempData["UserId"]?.ToString();
            _logger.LogInformation($"RemoveFromCart called. Session UserId: {userIdStr ?? "null"}, CartItemId: {cartItemId}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session.");
                return Json(new { success = false, message = "Please log in to modify your cart." });
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                _logger.LogWarning($"CartItemId {cartItemId} not found for UserId {userId}.");
                return Json(new { success = false, message = "Item not found in cart." });
            }

            // Restore stock quantity
            cartItem.Product.StockQuantity += cartItem.Quantity;
            _context.Products.Update(cartItem.Product);

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Removed CartItemId {cartItemId} from cart for UserId {userId}. Stock restored to {cartItem.Product.StockQuantity}");

            return Json(new { success = true, message = "Item removed from cart." });
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userIdStr = HttpContext.Session.GetString("UserId") ?? TempData["UserId"]?.ToString();
            _logger.LogInformation($"UpdateQuantity called. Session UserId: {userIdStr ?? "null"}, CartItemId: {cartItemId}, Quantity: {quantity}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session.");
                return Json(new { success = false, message = "Please log in to modify your cart." });
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                _logger.LogWarning($"CartItemId {cartItemId} not found for UserId {userId}.");
                return Json(new { success = false, message = "Item not found in cart." });
            }

            if (quantity <= 0)
            {
                // Restore stock quantity
                cartItem.Product.StockQuantity += cartItem.Quantity;
                _context.Products.Update(cartItem.Product);

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Removed CartItemId {cartItemId} due to quantity <= 0 for UserId {userId}. Stock restored to {cartItem.Product.StockQuantity}");
                return Json(new { success = true, message = "Item removed from cart." });
            }

            if (quantity > cartItem.Product.StockQuantity + cartItem.Quantity)
            {
                _logger.LogWarning($"Cannot set quantity at or above available stock for CartItemId {cartItemId}. Requested: {quantity}, Available: {cartItem.Product.StockQuantity + cartItem.Quantity}");
                return Json(new { success = false, message = $"Out of Stock. Only {cartItem.Product.StockQuantity + cartItem.Quantity} items available." });
            }

            // Update stock: restore previous quantity, then deduct new quantity
            cartItem.Product.StockQuantity += cartItem.Quantity;
            cartItem.Product.StockQuantity -= quantity;
            _context.Products.Update(cartItem.Product);

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated CartItemId {cartItemId} quantity to {quantity} for UserId {userId}. Stock updated to {cartItem.Product.StockQuantity}");

            return Json(new { success = true, message = "Cart updated successfully." });
        }

        // GET: Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session.");
                TempData["ErrorMessage"] = "Please log in to proceed to checkout.";
                return RedirectToAction("Index", "Login");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                _logger.LogWarning($"No cart or empty cart for UserId {userId}.");
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            return View(cart);
        }

        [HttpGet]
        public IActionResult ConfirmOrder()
        {
            _logger.LogWarning("Direct GET request to ConfirmOrder. Redirecting to Checkout.");
            return RedirectToAction("Checkout");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder([FromForm] UserAddress address)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            _logger.LogInformation($"ConfirmOrder called. Session UserId: {userIdStr ?? "null"}, Address: AddressLine1={address.AddressLine1}, City={address.City}, State={address.State}, PostalCode={address.PostalCode}, Country={address.Country}, Phone={address.Phone}");

            int userId = 0;

            try
            {
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out userId))
                {
                    _logger.LogWarning("User not logged in or invalid UserId in session.");
                    return Json(new { success = false, message = "Please log in to confirm your order." });
                }

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    _logger.LogWarning($"No cart or empty cart for UserId {userId}.");
                    return Json(new { success = false, message = "Your cart is empty." });
                }

                if (string.IsNullOrEmpty(address.AddressLine1) || string.IsNullOrEmpty(address.City) ||
                    string.IsNullOrEmpty(address.State) || string.IsNullOrEmpty(address.PostalCode) ||
                    string.IsNullOrEmpty(address.Country) || string.IsNullOrEmpty(address.Phone))
                {
                    _logger.LogWarning($"Invalid address provided for UserId {userId}. AddressLine1: {address.AddressLine1}, City: {address.City}, State: {address.State}, PostalCode: {address.PostalCode}, Country: {address.Country}, Phone: {address.Phone}");
                    return Json(new { success = false, message = "Please fill in all required address fields." });
                }

                address.UserId = userId;
                address.AddressLine2 ??= string.Empty;
                _context.UserAddresses.Add(address);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Address saved with AddressId: {address.AddressId} for UserId {userId}");

                // إنشاء الـ Payment أولاً
                var payment = new Payment
                {
                    PaymentMethod = "Cash",
                    PaymentStatus = "Pending",
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentDate = DateTime.Now,
                    PaymentAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
                };

                // إنشاء الـ Order وربطه بالـ Payment
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = address.AddressId,
                    OrderDate = DateTime.Now,
                    TotalPrice = payment.PaymentAmount,
                    OrderStatus = "Pending",
                    PaymentStatus = "Pending",
                    PaymentMethod = "Cash",
                    TrackingNumber = "Pending",
                    ProductId = cart.CartItems.First().ProductId,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList(),
                    Payment = payment, // ربط الـ Payment هنا
                    PaymentId =  // حط null مؤقتاً
                };

                payment.Order = order; // ربط الـ Order بالـ Payment

                _context.Orders.Add(order);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order created with OrderId: {order.OrderId} and PaymentId: {payment.PaymentId} for UserId {userId}");

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Cart cleared for UserId {userId} after order confirmation");

                return Json(new { success = true, message = "Order confirmed successfully!", redirectUrl = Url.Action("Profile", "Account") });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error confirming order for UserId {UserId}. Inner Exception: {InnerExceptionMessage}, StackTrace: {StackTrace}", userId, dbEx.InnerException?.Message, dbEx.StackTrace);
                return Json(new { success = false, message = $"Database error: {dbEx.InnerException?.Message ?? "An error occurred while saving to the database. Please try again."}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error confirming order for UserId {UserId}. StackTrace: {StackTrace}, Inner Exception: {InnerExceptionMessage}", userId, ex.StackTrace, ex.InnerException?.Message);
                return Json(new { success = false, message = $"An unexpected error occurred: {ex.Message}. Please try again." });
            }
        }
    }
    }
