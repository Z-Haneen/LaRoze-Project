using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ShippingAddressId { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string TrackingNumber { get; set; }


        public Product Product { get; set; }
        public int ProductId { get; set; }
        public User User { get; set; }
        public UserAddress ShippingAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
        public int PaymentId { get; set; }
        

    }
}
