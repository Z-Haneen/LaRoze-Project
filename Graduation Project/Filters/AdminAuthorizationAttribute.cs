using Microsoft.AspNetCore.Mvc;

namespace Graduation_Project.Filters
{
    public class AdminAuthorizationAttribute : TypeFilterAttribute
    {
        public AdminAuthorizationAttribute() : base(typeof(AdminAuthorizationFilter))
        {
        }
    }
}