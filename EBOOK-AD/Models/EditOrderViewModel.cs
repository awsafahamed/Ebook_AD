using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{
    public class EditOrderViewModel
    {
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}
