// Models/ClaimModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_PROG6212_POE.Models
{
    public class ClaimModel
    {
        [Key]
        public int ClaimId { get; set; }

        [Required]
        [Range(0, 999)]
        public int HoursWorked { get; set; }

        [Required]
        [Range(0, 9999.99)]
        public decimal HourlyRate { get; set; }

        public string Notes { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        // Foreign Key
        [Required]
        public int UserId { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();

        public ApprovalModel Approval { get; set; } = new ApprovalModel();

        [NotMapped]
        public decimal TotalAmount => HoursWorked * HourlyRate;
    }
}