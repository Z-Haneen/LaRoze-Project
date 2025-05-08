using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graduation_Project.Models
{
    public class UserAddress
    {
        [Key]
        [Column("Id")]
        public int AddressId { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [NotMapped]
        [StringLength(100)]
        [Display(Name = "Address Name")]
        public string AddressName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [StringLength(20)]
        public string Phone { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Street address is required")]
        [Display(Name = "Street Address")]
        [StringLength(200)]
        [Column("AddressLine1")]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        [Display(Name = "Apartment, suite, etc.")]
        [Column("AddressLine2")]
        public string? ApartmentNumber { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "State/Province is required")]
        [StringLength(100)]
        [Display(Name = "State/Province")]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        public string Country { get; set; }

        [Display(Name = "Default Address")]
        public bool IsDefault { get; set; }

        [NotMapped]
        [Display(Name = "Address Type")]
        public string AddressType { get; set; } // Home, Work, etc.

        [NotMapped]
        [StringLength(500)]
        [Display(Name = "Delivery Instructions")]
        public string DeliveryInstructions { get; set; }
    }
}
