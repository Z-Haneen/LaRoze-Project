using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
