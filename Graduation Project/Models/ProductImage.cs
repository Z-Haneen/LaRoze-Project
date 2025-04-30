using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public bool DefaultImage { get; set; }

        public virtual Product Product { get; set; }
    }
}
