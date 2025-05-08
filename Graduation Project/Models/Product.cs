using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Graduation_Project.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }

        // Added properties for view compatibility
        [NotMapped]
        public string MainImage
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageUrl))
                    return ImageUrl;

                if (ProductImages != null && ProductImages.Any())
                    return ProductImages.FirstOrDefault()?.ImageUrl;

                return "/images/placeholder.jpg"; // Default placeholder
            }
        }

        [NotMapped]
        public int Stock
        {
            get { return StockQuantity; }
            set { StockQuantity = value; }
        }

        public Category Category { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<Promotion> Promotions { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}
