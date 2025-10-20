using CMCS_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        public IActionResult Index()
        {
            var pendingClaims = DataStore.Claims.Where(c => c.Status == "Pending").ToList();
            return View(pendingClaims);
        }
        [HttpPost]
        public IActionResult Index(int claimId, string action)
        {
            var claim = DataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim != null)
            {
                claim.Approval.CoordinatorId = 1; // Mock ID
                claim.Status = action == "Verify" ? "Verified" : "Rejected";
                return RedirectToAction("Index");
            }
            return View(DataStore.Claims.Where(c => c.Status == "Pending").ToList());
        }
    }
}
