using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }
        [Required, StringLength(100)]
        public string SubCategoryName { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }

        public virtual Category Category { get; set; }

    }
}
