using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Graduation_Project.Controllers
{
    public class WishlistController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(GraduationDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Wishlist
        public async Task<IActionResult> Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var wishlistItems = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .ToListAsync();

            return View(wishlistItems);
        }

        // POST: Wishlist/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Check if already in wishlist
            var existingItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem == null)
            {
                // Add to wishlist
                var wishlistItem = new Wishlist
                {
                    UserId = userId,
                    ProductId = productId,
                    DateAdded = DateTime.Now
                };

                _context.Wishlists.Add(wishlistItem);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product added to your wishlist.";
            }
            else
            {
                TempData["InfoMessage"] = "This product is already in your wishlist.";
            }

            // Redirect back to the product page or wherever the request came from
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        // POST: Wishlist/Remove/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.WishlistId == id && w.UserId == userId);

            if (wishlistItem == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product removed from your wishlist.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Wishlist/MoveToCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToCart(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            // Find the wishlist item
            var wishlistItem = await _context.Wishlists
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.WishlistId == id && w.UserId == userId);

            if (wishlistItem == null)
            {
                return NotFound();
            }

            // Check if the user already has a cart
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                // Create a new cart if one doesn't exist
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the product is already in the cart
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == wishlistItem.ProductId);

            if (cartItem == null)
            {
                // Add the product to the cart
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = wishlistItem.ProductId,
                    Quantity = 1,
                    AddedAt = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Increment the quantity if the product is already in the cart
                cartItem.Quantity += 1;
                _context.CartItems.Update(cartItem);
            }

            // Remove the item from the wishlist
            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product moved to your cart.";
            return RedirectToAction(nameof(Index));
        }
    }
}