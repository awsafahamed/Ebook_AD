using System.ComponentModel.DataAnnotations;

namespace EBOOK_AD.Models
{

    public class Report
    {
        [Key]
        public int ReportId { get; set; }   
        public string ReportType { get; set; } // e.g., Sales, Orders, Users
        public int TotalRecords { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}