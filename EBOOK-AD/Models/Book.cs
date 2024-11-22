using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }
    }
}