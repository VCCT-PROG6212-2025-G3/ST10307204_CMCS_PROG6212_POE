using CMCS_PROG6212_POE.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CMCS_PROG6212_POE.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }

        [Required]
        public int UserId { get; set; }

        
        public User? User { get; set; }

        [Required(ErrorMessage = "Hours worked is required.")]
        [Range(0.1, 9999.99, ErrorMessage = "Hours worked must be greater than 0.")]
        public decimal HoursWorked { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public decimal TotalAmount => Math.Round(HoursWorked * HourlyRate, 2);

        public List<DocumentModel> Documents { get; set; } = new();
        public ApprovalModel? Approval { get; set; }
    }
}
