using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace Graduation_Project.Controllers
{
    public class RegisterController : Controller
    {
        private readonly GraduationDbContext _context;

        public RegisterController(GraduationDbContext context)
        {
            _context = context;
        }
        
        // GET: Register/Index - Add this method to handle the link from login page
        public IActionResult Index()
        {
            return RedirectToAction("Add");
        }
        
        // GET: عرض صفحة التسجيل
        //  /Register/Add
        public IActionResult Add()
        {
            return View("Add"); // نموذج فارغ للعرض
        }

        // POST: حفظ بيانات المستخدم الجديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAdd(User newUserFromRequest)
        {
            // التحقق من صحة البيانات
            if (!string.IsNullOrWhiteSpace(newUserFromRequest.FirstName) &&
                !string.IsNullOrWhiteSpace(newUserFromRequest.LastName) &&
                !string.IsNullOrWhiteSpace(newUserFromRequest.Email) &&
                !string.IsNullOrWhiteSpace(newUserFromRequest.Password))
            {
                // Check if email already exists
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == newUserFromRequest.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View("Add", newUserFromRequest);
                }
                
                // Continue with registration
                // Hash password before storing
                newUserFromRequest.Password = BCrypt.Net.BCrypt.HashPassword(newUserFromRequest.Password);
                
                // تعيين القيم الأساسية والافتراضية
                newUserFromRequest.RegistrationDate = DateTime.Now; // تاريخ التسجيل الآن
                newUserFromRequest.Status = "Active"; // حالة المستخدم
                newUserFromRequest.LastLogin = default(DateTime); // لم يقم بتسجيل الدخول بعد
                newUserFromRequest.RoleId = 2; // Default role ID for regular users
                
                // إضافة المستخدم إلى قاعدة البيانات
                _context.Users.Add(newUserFromRequest);
                _context.SaveChanges();

                // رسالة نجاح
                TempData["SuccessMessage"] = "Registration is done successfully.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                // إضافة رسائل خطأ إذا كانت البيانات غير صالحة
                ModelState.AddModelError("FirstName", "الاسم الأول مطلوب.");
                ModelState.AddModelError("LastName", "الاسم الأخير مطلوب.");
                ModelState.AddModelError("Email", "البريد الإلكتروني مطلوب.");
                ModelState.AddModelError("Password", "كلمة المرور مطلوبة.");
                return View("Add", newUserFromRequest);
            }
        }
    }
}
