using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        [Required, StringLength(200)]
        public string Subject { get; set; }
        public string Message { get; set; }

        // Foreign key for User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
