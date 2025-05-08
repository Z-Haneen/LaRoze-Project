using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Graduation_Project.Models
{
    public class Order
    {
        [Key]
        [Column("Id")] // Map to the actual column name in the database
        public int OrderId { get; set; }

        public int UserId { get; set; }

        [Column("ShippingAddressId")]
        public int? ShippingAddressId { get; set; }

        [Column("TotalAmount")]
        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; }

        // Status is an int in the database
        [Column("Status")]
        public int StatusId { get; set; }

        [NotMapped] // This will be used for backward compatibility
        public string Status
        {
            get { return GetStatusString(StatusId); }
            set { StatusId = GetStatusId(value); }
        }

        [NotMapped]
        public string OrderStatus
        {
            get { return GetStatusString(StatusId); }
            set { StatusId = GetStatusId(value); }
        }

        [NotMapped]
        public string PaymentStatus { get; set; } = "Pending";

        [NotMapped]
        public string PaymentMethod { get; set; } = "Cash on Delivery";

        public string? TrackingNumber { get; set; }

        [NotMapped]
        public DateTime? DeliveryDate
        {
            get { return DeliveredDate; }
            set { DeliveredDate = value; }
        }

        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }

        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }

        // Shipping information properties
        [NotMapped]
        public string? ShippingName { get; set; }
        [NotMapped]
        public string? ShippingAddress { get; set; }
        [NotMapped]
        public string? ShippingCity { get; set; }
        [NotMapped]
        public string? ShippingState { get; set; }
        [NotMapped]
        public string? ShippingPostalCode { get; set; }
        [NotMapped]
        public string? ShippingCountry { get; set; }
        [NotMapped]
        public string? ShippingPhone { get; set; }

        // Order summary properties
        [NotMapped]
        public decimal SubTotal
        {
            get
            {
                decimal subtotal = 0;
                if (OrderItems != null)
                {
                    foreach (var item in OrderItems)
                    {
                        subtotal += item.Price * item.Quantity;
                    }
                }
                return subtotal;
            }
        }

        [NotMapped]
        public decimal ShippingCost { get; set; } = 0;

        [NotMapped]
        public decimal Discount { get; set; } = 0;

        [NotMapped] // This will be used for backward compatibility with views
        public decimal TotalAmount
        {
            get { return TotalPrice; }
            set { TotalPrice = value; }
        }

        [NotMapped]
        public int ProductId { get; set; }

        [NotMapped]
        public Product? Product { get; set; }

        public User User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int? PaymentId { get; set; }

        [ForeignKey("PaymentId")]
        public Payment? Payment { get; set; }

        // Remove circular reference
        // public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // Helper methods to convert between string status and int status
        private string GetStatusString(int statusId)
        {
            return statusId switch
            {
                0 => "Pending",
                1 => "Processing",
                2 => "Shipped",
                3 => "Delivered",
                4 => "Cancelled",
                _ => "Unknown"
            };
        }

        private int GetStatusId(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => 0,
                "processing" => 1,
                "shipped" => 2,
                "delivered" => 3,
                "cancelled" => 4,
                _ => 0 // Default to Pending
            };
        }
    }
}
