    using System.Collections.Generic;
    using CMCS_PROG6212_POE.Models;

namespace CMCS_PROG6212_POE.Data
{
   

    public static class DataStore
    {
        public static List<ClaimModel> Claims { get; set; } = new List<ClaimModel>();
        public static List<DocumentModel> Documents { get; set; } = new List<DocumentModel>();
        public static List<ApprovalModel> Approvals { get; set; } = new List<ApprovalModel>();
    }
}
