using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Models;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Graduation_Project.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<PasswordResetController> _logger;

        public PasswordResetController(GraduationDbContext context, ILogger<PasswordResetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: PasswordReset/RequestReset
        public IActionResult RequestReset()
        {
            return View();
        }

        // POST: PasswordReset/RequestReset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestReset(PasswordResetRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                
                if (user != null)
                {
                    // Generate a reset token (in a real app, this would be more secure)
                    string token = GenerateResetToken();
                    
                    // Store the token in TempData (in a real app, you'd store this in the database)
                    // with an expiration time
                    TempData["ResetToken"] = token;
                    TempData["ResetEmail"] = model.Email;
                    
                    // In a real application, you would send an email with a reset link
                    // For this demo, we'll just redirect to the reset page with the token
                    
                    _logger.LogInformation($"Password reset requested for {model.Email}. Token: {token}");
                    
                    // Simulate email sending
                    TempData["SuccessMessage"] = "Password reset link has been sent to your email.";
                    
                    // For demo purposes, redirect directly to reset page
                    return RedirectToAction("ResetPassword", new { token = token });
                }
                
                // Don't reveal that the user doesn't exist
                TempData["SuccessMessage"] = "If your email is registered, you will receive a password reset link.";
                return View();
            }
            
            return View(model);
        }

        // GET: PasswordReset/ResetPassword
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token) || TempData["ResetToken"] == null || token != (string)TempData["ResetToken"])
            {
                TempData["ErrorMessage"] = "Invalid or expired password reset token.";
                return RedirectToAction("RequestReset");
            }
            
            // Keep the token in TempData for the POST action
            TempData.Keep("ResetToken");
            TempData.Keep("ResetEmail");
            
            var model = new PasswordResetModel { Token = token };
            return View(model);
        }

        // POST: Process password reset
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(PasswordResetModel model)
        {
            if (ModelState.IsValid)
            {
                // Verify token
                if (TempData["ResetToken"] == null || model.Token != (string)TempData["ResetToken"])
                {
                    TempData["ErrorMessage"] = "Invalid or expired password reset token.";
                    return RedirectToAction("RequestReset");
                }
                
                string email = (string)TempData["ResetEmail"];
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                
                if (user != null)
                {
                    // Update password
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    _context.SaveChanges();
                    
                    TempData["SuccessMessage"] = "Your password has been reset successfully. You can now log in with your new password.";
                    return RedirectToAction("Index", "Login");
                }
                
                TempData["ErrorMessage"] = "An error occurred while resetting your password.";
                return RedirectToAction("RequestReset");
            }
            
            return View(model);
        }

        // Helper method to generate a random token
        private string GenerateResetToken()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16);
        }
    }
}


