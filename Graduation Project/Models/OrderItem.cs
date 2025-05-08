using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graduation_Project.Models
{
    public class OrderItem
    {
        [Key]
        [Column("Id")]
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [NotMapped]
        public decimal TotalPrice { get; set; }

        [NotMapped]
        public string? SelectedSize { get; set; }

        public string? ProductName { get; set; }

        public string? ProductSku { get; set; }

        public string? ProductImage { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
