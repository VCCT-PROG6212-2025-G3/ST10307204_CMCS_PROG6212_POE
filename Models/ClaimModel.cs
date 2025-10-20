using System.ComponentModel.DataAnnotations;
using CMCS_PROG6212_POE.Models;

public class ClaimModel
{
    public int ClaimId { get; set; }
    [Required]
    [Range(0, 999)]
    public int HoursWorked { get; set; }
    [Required]
    [Range(0, 9999.99)]
    public decimal HourlyRate { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; } = "Pending";
    public LecturerModel Lecturer { get; set; } = new LecturerModel();
    public List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
    public ApprovalModel Approval { get; set; } = new ApprovalModel(); // Ensure initialized
}