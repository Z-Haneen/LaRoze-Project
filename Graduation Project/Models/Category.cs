using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }

    }
}