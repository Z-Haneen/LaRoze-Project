using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Graduation_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(GraduationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                TempData["ErrorMessage"] = "Please log in to view your profile.";
                return RedirectToAction("Index", "Login");
            }

            var user = await _context.Users
                .Include(u => u.UserAddresses)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Login");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var addresses = await _context.UserAddresses
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                User = user,
                Addresses = addresses,
                Orders = orders
            };

            return View(viewModel);
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/GetOrderDetails
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Please log in to view order details." });
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            var orderItems = order.OrderItems.Select(oi => new
            {
                productName = oi.Product.Name,
                price = oi.Product.Price,
                quantity = oi.Quantity
            }).ToList();

            return Json(new { success = true, orderItems });
        }
    }
}