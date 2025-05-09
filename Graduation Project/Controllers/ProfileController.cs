using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Graduation_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(GraduationDbContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Profile
        public IActionResult Index()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = int.Parse(userIdStr);
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Profile/Edit
        public IActionResult Edit()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Index", "Login");
            }

            int userId = int.Parse(userIdStr);
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || int.Parse(userIdStr) != user.UserId)
            {
                return RedirectToAction("Index", "Login");
            }

            // Get the current user from the database
            var currentUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.UserId == user.UserId);
            if (currentUser == null)
            {
                return NotFound();
            }

            // Check if email is changed and if it's already in use
            if (currentUser.Email != user.Email)
            {
                var emailExists = _context.Users.Any(u => u.Email == user.Email && u.UserId != user.UserId);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already in use by another account.");
                    return View(user);
                }
            }

            // Preserve password and other fields that shouldn't be updated here
            user.Password = currentUser.Password;
            user.RegistrationDate = currentUser.RegistrationDate;
            user.LastLogin = currentUser.LastLogin;
            user.Status = currentUser.Status;

            // We don't need to preserve DateOfBirth and Gender as they are part of the form now

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    _context.SaveChanges();

                    // Update session data
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("UserEmail", user.Email);

                    TempData["SuccessMessage"] = "Your profile has been updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Error updating user profile for UserId: {UserId}", user.UserId);
                        ModelState.AddModelError("", "An error occurred while saving your profile. Please try again later.");
                        return View(user);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error updating user profile for UserId: {UserId}", user.UserId);
                    ModelState.AddModelError("", "An error occurred while saving your profile. Please try again later.");
                    return View(user);
                }
            }
            return View(user);
        }

        // GET: Profile/ChangePassword
        public IActionResult ChangePassword()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Index", "Login");
            }

            var model = new ChangePasswordModel
            {
                UserId = int.Parse(userIdStr)
            };

            return View(model);
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || int.Parse(userIdStr) != model.UserId)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                try
            {
                var user = _context.Users.Find(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(model);
                }

                // Update password
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error changing password for UserId: {UserId}", model.UserId);
                    ModelState.AddModelError("", "An error occurred while changing your password. Please try again later.");
                }
            }

            return View(model);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
