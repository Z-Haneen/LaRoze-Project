using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Models;
using Microsoft.Extensions.Logging;

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

        // GET: Display login page
        public IActionResult Index()
        {
            return View("Login");
        }

        // POST: Handle login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);

                if (user != null && user.Password == loginModel.Password)
                {
                    user.LastLogin = DateTime.Now;
                    _context.SaveChanges();

                    // Set session data
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserRole", user.RoleId.ToString());
                    _logger.LogInformation($"User {user.UserId} logged in successfully. Session UserId set to {user.UserId}");

                    // Log session keys and cookie for debugging
                    var sessionKeys = HttpContext.Session.Keys;
                    var sessionCookie = HttpContext.Request.Cookies["ASP.NET_SessionId"];
                    _logger.LogInformation($"After setting session. Session Keys: {string.Join(", ", sessionKeys)}, Session Cookie: {sessionCookie ?? "null"}");

                    // Ensure session is committed before redirect
                    HttpContext.Session.CommitAsync().GetAwaiter().GetResult();

                    TempData["SuccessMessage"] = "Login successful! Redirecting to home.";
                    return RedirectToAction("Index", "Home");
                }

                // Handle invalid login
                TempData["ErrorMessage"] = "Invalid email or password.";
                _logger.LogWarning($"Failed login attempt for email: {loginModel.Email}");
            }
            else
            {
                TempData["ErrorMessage"] = "Please provide valid email and password.";
            }

            return View("Login", loginModel);
        }

        // GET: Handle logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            _logger.LogInformation("User logged out. Session cleared.");
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Login");
        }
    }
}