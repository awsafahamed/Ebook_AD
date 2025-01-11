using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBOOK_AD.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }  // Foreign key for User

        [Required]
        public decimal TotalAmount { get; set; }  // Total price for the order

        [Required]
        public DateTime OrderDate { get; set; }  // Date when the order was placed

        [Required]
        [StringLength(50)]
        public string Status { get; set; }  // Order status (e.g., Pending, Completed, Cancelled)

        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; }  // Shipping address for the order

        // Navigation property for related OrderItems
        public virtual List<OrderItem> OrderItems { get; set; }  // One-to-many relationship with OrderItem

        // Optional: If you want to include more info like payment status or delivery date
        [StringLength(50)]
        public string PaymentStatus { get; set; }  // Payment status (e.g., Paid, Pending, Failed)

        public DateTime? DeliveryDate { get; set; }  // Optional: Date when the order is delivered (nullable)
    }

    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }  // Foreign Key linking to Order

        [Required]
        public int BookId { get; set; }  // ID of the book being ordered

        [Required]
        public int Quantity { get; set; }  // Quantity of the book

        [Required]
        public decimal Price { get; set; }  // Price per book item

        // Navigation property linking back to Order
        public virtual Order Order { get; set; }

          public virtual Book Book { get; set; }

        // Optional: You can also add a Book navigation property if you want to load book details
        // public virtual Book Book { get; set; }  // Assuming you have a Book model
    }
}
