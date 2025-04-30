using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graduation_Project.Models
{
    public class ProductDto
    {
        [Required(ErrorMessage = "Product Name is required.")]
        [MaxLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [MaxLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
        public string Sku { get; set; } = "";

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "";

        // Image Upload Handling
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}