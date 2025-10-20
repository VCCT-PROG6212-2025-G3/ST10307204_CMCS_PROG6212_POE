using CMCS_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            var pendingClaims = DataStore.Claims.Where(c => c.Status == "Verified").ToList();
            return View(pendingClaims);
        }
        [HttpPost]
        public IActionResult Index(int claimId, string action)
        {
            var claim = DataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Approval.ManagerId = 1; // Mock ID
                claim.Approval.ApprovalDate = DateTime.Now;
                claim.Status = action == "Approve" ? "Approved" : "Rejected";
                return RedirectToAction("Index");
            }
            return View(DataStore.Claims.Where(c => c.Status == "Verified").ToList());
        }
    }
}
