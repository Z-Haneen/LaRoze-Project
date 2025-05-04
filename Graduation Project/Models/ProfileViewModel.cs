namespace Graduation_Project.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<UserAddress> Addresses { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}