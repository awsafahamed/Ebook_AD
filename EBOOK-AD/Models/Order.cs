using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Pending, Completed, Cancelled

        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        // Navigation property
        public virtual List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }  // Foreign Key

        [Required]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Navigation property to related Order
        public virtual Order Order { get; set; }
    }
}
