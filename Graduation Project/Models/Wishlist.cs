using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
