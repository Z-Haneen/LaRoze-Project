using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Graduation_Project.Models;

namespace Graduation_Project.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; } // Added this property

    }
}


