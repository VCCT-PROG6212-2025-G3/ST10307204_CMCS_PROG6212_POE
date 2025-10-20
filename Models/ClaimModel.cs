using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class ClaimModel
    {
        public int ClaimId { get; set; }
        public int LecturerId { get; set; }
        [Required]
        [Range(0, 999)]
        public int HoursWorked { get; set; }
        [Required]
        [Range(0, 9999.99)]
        public decimal HourlyRate { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending";
        public List<DocumentModel> Documents { get; set; } = new();
        public ApprovalModel Approval { get; set; }
    }
}

public class ClaimModel
{
    
}