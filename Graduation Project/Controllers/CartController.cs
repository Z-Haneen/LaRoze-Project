using Graduation_Project.Models;
using Graduation_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
                HttpContext.Session.SetString("UserId", userIdStr); // Persist TempData UserId into session
                await HttpContext.Session.CommitAsync(); // Ensure session is saved
                TempData.Remove("UserId"); // Clear TempData to avoid reuse
            }

            var sessionKeys = HttpContext.Session.Keys;
            var sessionCookie = HttpContext.Request.Cookies["ASP.NET_SessionId"];
            _logger.LogInformation($"Cart/Index called. Session UserId: {userIdStr ?? "null"}, Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}");

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                _logger.LogWarning("User not logged in or invalid UserId in session. Falling back to TempData failed.");
                TempData["ErrorMessage"] = "Please log in to view your cart.";
                return RedirectToAction("Index", "Login"); // Redirect to login page
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                _logger.LogInformation($"No cart found for UserId {userId}. Creating new cart.");
                cart = new Models.Cart { UserId = userId, CreatedAt = DateTime.Now, CartItems = new List<Models.CartItem>() };
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

            if (quantity > product.StockQuantity) // غيرنا >= إلى >
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
                cart = new Models.Cart { UserId = userId, CreatedAt = DateTime.Now, CartItems = new List<Models.CartItem>() };
                _context.Carts.Add(cart);
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                var newQuantity = cartItem.Quantity + quantity;
                var availableQuantity = product.StockQuantity + cartItem.Quantity; // نجمع الكمية الموجودة في السلة مع الكمية المتاحة
                if (newQuantity > availableQuantity)
                {
                    _logger.LogWarning($"Cannot add more items than available stock for ProductId {productId}. Current: {cartItem.Quantity}, Requested: {quantity}, Available: {availableQuantity}");
                    return Json(new { success = false, message = $"Out of Stock. Only {availableQuantity} items available." });
                }
                cartItem.Quantity = newQuantity;
            }
            else
            {
                cartItem = new Models.CartItem { CartId = cart.CartId, ProductId = productId, Quantity = quantity };
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

            if (quantity > cartItem.Product.StockQuantity + cartItem.Quantity) // غيرنا >= إلى >
            {
                _logger.LogWarning($"Cannot set quantity at or above available stock for CartItemId {cartItemId}. Requested: {quantity}, Available: {cartItem.Product.StockQuantity + cartItem.Quantity}");
                return Json(new { success = false, message = $"Out of Stock. Only {cartItem.Product.StockQuantity + cartItem.Quantity} items available." });
            }

            // Update stock: restore previous quantity, then deduct new quantity
            cartItem.Product.StockQuantity += cartItem.Quantity; // Restore old quantity
            cartItem.Product.StockQuantity -= quantity; // Deduct new quantity
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

            // Get user addresses
            var addresses = await _context.UserAddresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.FullName)
                .ToListAsync();

            // Create a view model with cart and addresses
            var viewModel = new ViewModels.CheckoutViewModel
            {
                Cart = cart,
                Addresses = addresses
            };

            return View(viewModel);
        }

        // POST: Cart/ConfirmOrder
        [HttpPost]
        [ValidateAntiForgeryToken(Order = 1000)] // Lower order to make it run after model binding
        public async Task<IActionResult> ConfirmOrder(int? addressId)
        {
            _logger.LogInformation($"ConfirmOrder called with addressId: {addressId}");
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
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

            // Validate that an address was selected
            if (!addressId.HasValue)
            {
                return Json(new { success = false, message = "Please select a shipping address before placing your order." });
            }

            // Get shipping address
            var shippingAddress = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId.Value && a.UserId == userId);

            if (shippingAddress == null)
            {
                _logger.LogWarning($"Address with ID {addressId} not found for UserId {userId}.");
                return Json(new { success = false, message = "The selected shipping address was not found. Please select a valid address." });
            }

            try
            {
                // Create payment record first
                var payment = new Models.Payment
                {
                    PaymentMethod = "Cash on Delivery",
                    PaymentStatus = "Pending",
                    Amount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                    PaymentDate = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Create new order with payment ID
                var order = new Models.Order
                {
                    UserId = userId,
                    ShippingAddressId = shippingAddress.AddressId, // Use AddressId instead of Id
                    OrderDate = DateTime.Now,
                    Status = "Pending", // This will set StatusId to 0
                    TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price),
                    TrackingNumber = GenerateTrackingNumber(),
                    PaymentId = payment.PaymentId,
                    OrderItems = new List<Models.OrderItem>()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Update payment with order ID
                payment.OrderId = order.OrderId;
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order or payment: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your order. Please try again." });
            }

            try
            {
                // Get the order we just created
                var order = await _context.Orders
                    .OrderByDescending(o => o.OrderId)
                    .FirstOrDefaultAsync(o => o.UserId == userId);

                if (order == null)
                {
                    _logger.LogError("Order not found after creation");
                    return Json(new { success = false, message = "An error occurred while processing your order. Please try again." });
                }

                // Add order items
                foreach (var cartItem in cart.CartItems)
                {
                    var orderItem = new Models.OrderItem
                    {
                        OrderId = order.OrderId, // Set the correct OrderId
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Product.Price,
                        TotalPrice = cartItem.Quantity * cartItem.Product.Price,
                        ProductName = cartItem.Product.Name ?? "Unknown Product",
                        ProductSku = cartItem.Product.Sku ?? "",
                        ProductImage = cartItem.Product.ImageUrl ?? ""
                    };

                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync();

                // Clear the cart
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order #{order.OrderId} confirmed for UserId {userId}. Cart cleared.");

                return Json(new { success = true, message = "Order confirmed successfully!", redirectUrl = Url.Action("Profile", "Account") });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order items: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while processing your order. Please try again." });
            }
        }

        // Helper method to generate a tracking number
        private string GenerateTrackingNumber()
        {
            return "TRK" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        }
    }
}
