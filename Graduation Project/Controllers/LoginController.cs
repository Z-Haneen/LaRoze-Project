using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Graduation_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly GraduationDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(GraduationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Login
        public IActionResult Index()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Role) // Include Role to access role information
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // Update last login time
                    user.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Set session variables
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.RoleId.ToString());
                    HttpContext.Session.SetString("UserRoleName", user.Role?.Name ?? "User");

                    _logger.LogInformation($"User {user.Email} logged in successfully");

                    // Redirect to home page
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email or password");
                _logger.LogWarning($"Failed login attempt for email: {model.Email}");
            }

            return View(model);
        }

        // For backward compatibility with Login.cshtml
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Just redirect to the Index action with the same model
            return await Index(model);
        }

        // GET: Login/Login
        public IActionResult Login()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // GET: Login/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
