using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PromotionType { get; set; }

        public Product Product { get; set; }
    }
}
