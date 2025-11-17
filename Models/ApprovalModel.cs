// Models/ApprovalModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_PROG6212_POE.Models
{
    public class ApprovalModel
    {
        [Key]
        public int ApprovalId { get; set; }

        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public ClaimModel Claim { get; set; } = null!;

        public int? VerifiedById { get; set; }
        [ForeignKey("VerifiedById")]
        public User? VerifiedBy { get; set; }
        public DateTime? VerifiedDate { get; set; }

        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public User? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}