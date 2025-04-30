using Graduation_Project.Models;
using Microsoft.AspNetCore.Mvc;
//using BCrypt.Net;

namespace Graduation_Project.Controllers
{
    public class RegisterController : Controller
    {
        private readonly GraduationDbContext _context;

        public RegisterController(GraduationDbContext context)
        {
            _context = context;
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
               // newUserFromRequest.Password = BCrypt.HashPassword(newUserFromRequest.Password);
                // تعيين القيم الأساسية والافتراضية
                newUserFromRequest.RegistrationDate = DateTime.Now; // تاريخ التسجيل الآن
                newUserFromRequest.Status = "Active"; // حالة المستخدم
                newUserFromRequest.LastLogin = default(DateTime); // لم يقم بتسجيل الدخول بعد
                
                // إضافة المستخدم إلى قاعدة البيانات
                _context.Users.Add(newUserFromRequest);
                _context.SaveChanges();

                // رسالة نجاح
                TempData["SuccessMessage"] = "Registration is done successfully.";
                return RedirectToAction("Index", "Home");
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