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
    public class OrderController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(GraduationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Order/History
        public async Task<IActionResult> History()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order != null && order.ShippingAddressId.HasValue)
            {
                // Get the shipping address
                var shippingAddress = await _context.UserAddresses
                    .FirstOrDefaultAsync(a => a.AddressId == order.ShippingAddressId.Value);

                if (shippingAddress != null)
                {
                    // Populate the shipping address fields
                    order.ShippingName = shippingAddress.FullName;
                    order.ShippingAddress = shippingAddress.StreetAddress; // Use StreetAddress instead of AddressLine1
                    order.ShippingCity = shippingAddress.City;
                    order.ShippingState = shippingAddress.State;
                    order.ShippingPostalCode = shippingAddress.PostalCode;
                    order.ShippingCountry = shippingAddress.Country;
                    order.ShippingPhone = shippingAddress.PhoneNumber;
                }
            }

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            // Only allow cancellation if order is pending or processing
            if (order.Status == "Pending" || order.Status == "Processing")
            {
                order.Status = "Cancelled";

                // Update payment record if exists
                if (order.PaymentId.HasValue)
                {
                    var payment = await _context.Payments.FindAsync(order.PaymentId.Value);
                    if (payment != null)
                    {
                        payment.PaymentStatus = "Cancelled";
                        _context.Update(payment);
                    }
                }

                _context.Update(order);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "This order cannot be cancelled.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}

