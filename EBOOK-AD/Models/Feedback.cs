using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        [Required]
        [Range(1, 5)]  // Rating between 1 and 5
        public int Rating { get; set; }
    }
}
