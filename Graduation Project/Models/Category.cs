using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Graduation_Project.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; }
        
        public int? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }
        public ICollection<Category> ChildCategories { get; set; } // Added for self-referencing relationship
    }
}