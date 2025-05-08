using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Graduation_Project.Models
{
    public class Payment
    {
        [Key]
        [Column("Id")]
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        [NotMapped]
        public int UserId { get; set; }

        public string PaymentMethod { get; set; } = null!;

        [Column("Status")]
        public string PaymentStatus { get; set; } = null!;

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentDetails { get; set; }

        // Added for view compatibility
        [NotMapped]
        public string Status
        {
            get { return PaymentStatus; }
            set { PaymentStatus = value; }
        }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [NotMapped]
        public User? User { get; set; }

        // Remove circular reference
        // public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
