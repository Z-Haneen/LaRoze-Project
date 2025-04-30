using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
