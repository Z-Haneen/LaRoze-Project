using Graduation_Project.Models;
using Graduation_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly GraduationDbContext _context;

        public AccountController(GraduationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Profile
        public IActionResult Profile()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Create a simplified view model with only the user data
            var viewModel = new AccountProfileViewModel
            {
                User = user,
                // We'll handle orders and addresses in the view
                RecentOrders = new List<Order>(),
                Addresses = new List<UserAddress>()
            };

            // Get counts for dashboard
            ViewBag.OrderCount = _context.Orders.Count(o => o.UserId == userId);
            ViewBag.AddressCount = _context.UserAddresses.Count(a => a.UserId == userId);
            ViewBag.WishlistCount = _context.Wishlists.Count(w => w.UserId == userId);

            return View(viewModel);
        }

        // GET: Account/EditProfile
        public IActionResult EditProfile()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(Models.User user)
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));

            if (userId != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the current user from the database
                    var currentUser = _context.Users.Find(userId);

                    if (currentUser == null)
                    {
                        return NotFound();
                    }

                    // Update only the fields that can be changed by the user
                    currentUser.FirstName = user.FirstName;
                    currentUser.LastName = user.LastName;
                    currentUser.Phone = user.Phone;
                    currentUser.Gender = user.Gender;
                    currentUser.DateOfBirth = user.DateOfBirth;

                    _context.Update(currentUser);
                    _context.SaveChanges();

                    // Update session data
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");

                    TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(user);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ViewModels.ChangePasswordViewModel model)
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var user = _context.Users.Find(userId);

                if (user == null)
                {
                    return NotFound();
                }

                // Verify current password
                bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password);
                if (!isCurrentPasswordValid)
                {
                    TempData["ErrorMessage"] = "Current password is incorrect.";
                    return View(model);
                }

                // Update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.Update(user);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction(nameof(Profile));
            }

            return View(model);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }


}

