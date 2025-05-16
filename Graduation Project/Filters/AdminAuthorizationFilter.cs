using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Graduation_Project.Filters
{
    public class AdminAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roleId = context.HttpContext.Session.GetString("RoleId");
            
            // Check if user is not logged in or not an admin
            if (string.IsNullOrEmpty(roleId) || roleId != "1")
            {
                // Redirect to login page
                context.Result = new RedirectToActionResult("Login", "Login", null);
            }
        }
    }
}