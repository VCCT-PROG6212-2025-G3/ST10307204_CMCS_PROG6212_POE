namespace CMCS_PROG6212_POE.Models
{
    public class ApprovalModel
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public int? CoordinatorId { get; set; } // Nullable until set
        public int? ManagerId { get; set; } // Nullable until set
        public string? ApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}
