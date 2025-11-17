// Models/ClaimModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_PROG6212_POE.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        // Keep this as a calculated property (C# only)
        public decimal TotalAmount => Math.Round(HoursWorked * HourlyRate, 2);

        // Navigation
        public List<DocumentModel> Documents { get; set; } = new();
        public ApprovalModel? Approval { get; set; }
    }
}