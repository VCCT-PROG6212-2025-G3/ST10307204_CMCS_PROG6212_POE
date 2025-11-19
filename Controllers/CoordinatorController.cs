using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Filters;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    [AuthorizeRole("Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly AppDbContext _db;

        public CoordinatorController(AppDbContext db) => _db = db;

        // Dashboard
        public IActionResult Index()
        {
            var claims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Include(c => c.Approval)
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }

        // Return claim details for modal
        [HttpGet]
        public IActionResult GetClaimDetails(int id)
        {
            var claim = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Include(c => c.Approval)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null)
                return Content("<div class='alert alert-danger'>Claim not found.</div>");

            return PartialView("_ClaimDetailsPartial", claim);
        }

        // Verify or Reject claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(int claimId, string action)
        {
            try
            {
                var claim = _db.Claims
                    .Include(c => c.Approval)
                    .Include(c => c.Documents)
                    .Include(c => c.User)
                    .FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found or already processed.";
                    return RedirectToAction("Index");
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    TempData["Error"] = "Session expired. Please log in again.";
                    return RedirectToAction("Index");
                }

                if (claim.Approval == null)
                    claim.Approval = new ApprovalModel();

                if (action == "Verify")
                {
                    claim.Status = "Verified";
                    claim.Approval.VerifiedById = userId.Value;
                    claim.Approval.VerifiedDate = DateTime.Now;
                    TempData["Success"] = "Claim verified and sent to Manager.";
                }
                else if (action == "Reject")
                {
                    claim.Status = "Rejected";
                    claim.Approval.VerifiedById = userId.Value;
                    claim.Approval.VerifiedDate = DateTime.Now;
                    TempData["Success"] = "Claim rejected.";
                }
                else
                {
                    TempData["Error"] = "Unknown action.";
                    return RedirectToAction("Index");
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // View model for AJAX verification
        public class VerifyClaimModel
        {
            public int claimId { get; set; }
            public string action { get; set; } = "";
        }
        [HttpGet]
        public IActionResult DownloadDocument(int documentId)
        {
            var doc = _db.Documents.FirstOrDefault(d => d.DocumentId == documentId);

            if (doc == null)
                return NotFound("Document not found.");

            // Use the encrypted filename stored in FilePath
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", doc.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist on the server.");

            // Return the file with the original name
            return PhysicalFile(filePath, "application/octet-stream", doc.FileName);
        }


    }
}
