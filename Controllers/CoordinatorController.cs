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
                claim.Approval.CoordinatorId = 1; // Mock ID; replace with session/user data in production
                switch (action)
                {
                    case "Verify":
                        claim.Status = "Verified";
                        TempData["SuccessMessage"] = $"Claim #{claimId} has been verified.";
                        break;
                    case "Reject":
                        claim.Status = "Rejected";
                        TempData["SuccessMessage"] = $"Claim #{claimId} has been rejected.";
                        break;
                    case "View":
                        TempData["SelectedClaimId"] = claimId;
                        return RedirectToAction("ViewClaim");
                    default:
                        TempData["ErrorMessage"] = "Invalid action selected.";
                        break;
                }
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Claim not found.";
            return View(DataStore.Claims.Where(c => c.Status == "Pending").ToList());
        }

        public IActionResult ViewClaim()
        {
            var claimId = TempData["SelectedClaimId"] as int?;
            if (!claimId.HasValue)
            {
                TempData["ErrorMessage"] = "No claim selected for viewing.";
                return RedirectToAction("Index");
            }

            var claim = DataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found or not pending.";
                return RedirectToAction("Index");
            }
            return View(claim);
        }
    }
}