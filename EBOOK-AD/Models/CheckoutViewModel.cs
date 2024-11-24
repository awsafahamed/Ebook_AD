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

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public decimal TotalAmount { get; set; }

        public int UserId { get; set; }
    }
}
