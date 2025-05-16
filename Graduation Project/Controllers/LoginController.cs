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
        public async Task<IActionResult> Index()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Check if admin user exists, if not create one
            var adminExists = await _context.Users.AnyAsync(u => u.Email == "admin@laroze.com");
            if (!adminExists)
            {
                // Check if admin role exists
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    // Create admin role
                    adminRole = new Role
                    {
                        Name = "Admin",
                        Description = "Administrator with full access"
                    };
                    _context.Roles.Add(adminRole);
                    await _context.SaveChangesAsync();
                }

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
                    RoleId = adminRole.RoleId
                };
                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created admin user: admin@laroze.com with password: Admin123!");
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

                if (user != null && !string.IsNullOrEmpty(user.Password) && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
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
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && !string.IsNullOrEmpty(user.Password) && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    // Update last login
                    user.LastLogin = DateTime.Now;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Store user info in session
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("RoleId", user.RoleId.ToString());
                    HttpContext.Session.SetString("RoleName", user.Role.Name);

                    // Redirect based on role
                    if (user.RoleId == 1) // Admin role
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            return View(model);
        }

        // GET: Login/Login
        public async Task<IActionResult> Login()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Check if admin user exists, if not create one
            var adminExists = await _context.Users.AnyAsync(u => u.Email == "admin@laroze.com");
            if (!adminExists)
            {
                // Check if admin role exists
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (adminRole == null)
                {
                    // Create admin role
                    adminRole = new Role
                    {
                        Name = "Admin",
                        Description = "Administrator with full access"
                    };
                    _context.Roles.Add(adminRole);
                    await _context.SaveChangesAsync();
                }

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
                    RoleId = adminRole.RoleId
                };
                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created admin user: admin@laroze.com with password: Admin123!");
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
