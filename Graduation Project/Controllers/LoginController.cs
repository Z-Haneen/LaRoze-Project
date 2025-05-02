using Microsoft.AspNetCore.Mvc;
using Graduation_Project.Models;

namespace Graduation_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly GraduationDbContext _context;

        public LoginController(GraduationDbContext context)
        {
            _context = context;
        }

        // GET: عرض صفحة تسجيل الدخول
        public IActionResult Index()
        {
            return View("Login");
        }

        // POST: تسجيل الدخول
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                // البحث عن المستخدم في قاعدة البيانات باستخدام البريد الإلكتروني
                var user = _context.Users.FirstOrDefault(u => u.Email == loginModel.Email);

                if (user != null)
                {
                    // التحقق من صحة كلمة المرور مباشرة (بدون تشفير)
                    if (user.Password == loginModel.Password)
                    {
                        // إذا تم العثور على المستخدم، تحديث حالة تسجيل الدخول
                        user.LastLogin = DateTime.Now;
                        _context.SaveChanges();

                        // تخزين معلومات المستخدم في Session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());
                        HttpContext.Session.SetString("UserRole", user.RoleId.ToString());

                        // إعادة توجيه إلى الصفحة الرئيسية
                        TempData["SuccessMessage"] = "Login successful!";
                        return Redirect("/Home/Index");
                    }
                }

                // إذا لم يتم العثور على المستخدم أو كلمة المرور غير صحيحة
                ModelState.AddModelError("", "Invalid email or password.");
            }

            // إذا كانت البيانات غير صالحة، إعادة عرض الصفحة مع رسائل الخطأ
            return View("Login", loginModel);
        }

        // GET: تسجيل الخروج
        public IActionResult Logout()
        {
            // مسح بيانات الجلسة
            HttpContext.Session.Clear();

            // إعادة توجيه إلى صفحة تسجيل الدخول
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Login");
        }
    }
}