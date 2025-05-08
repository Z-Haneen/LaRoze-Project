using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class PasswordResetRequestModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}


