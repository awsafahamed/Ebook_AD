using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{
    public class CheckoutViewModel
    {
        public List<OrderItem> OrderItems { get; set; }

        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [StringLength(200, ErrorMessage = "Billing address can't exceed 200 characters.")]
        public string BillingAddress { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
