namespace CMCS_PROG6212_POE.Models
{
    public class ApprovalModel
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public int? CoordinatorId { get; set; }
        public int? ManagerId { get; set; }
        public string ApprovalStatus { get; set; } = "Pending";
        public DateTime? ApprovalDate { get; set; }
    }
}
