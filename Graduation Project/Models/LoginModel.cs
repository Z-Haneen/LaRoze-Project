using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class LoginModel

    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}