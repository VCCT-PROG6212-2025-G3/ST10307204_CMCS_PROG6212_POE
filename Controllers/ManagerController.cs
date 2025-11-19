// Controllers/ManagerController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Filters;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    [AuthorizeRole("Manager")]
    public class ManagerController : Controller
    {
        private readonly AppDbContext _db;

        public ManagerController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var claims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Verified")
                .ToList();

            return View(claims);
        }

        [HttpPost]
        public IActionResult Approve(int claimId, string action)
        {
            var claim = _db.Claims
                .Include(c => c.Approval)
                .FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Verified");

            if (claim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (action == "Approve")
            {
                claim.Status = "Approved";
                claim.Approval.ApprovedById = userId;
                claim.Approval.ApprovedDate = DateTime.Now;
                TempData["Success"] = "Claim approved.";
            }
            else if (action == "Reject")
            {
                claim.Status = "Rejected";
                claim.Approval.ApprovedById = userId;
                claim.Approval.ApprovedDate = DateTime.Now;
                TempData["Success"] = "Claim rejected.";
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
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

        [HttpGet]
        public IActionResult DownloadDocument(int documentId)
        {
            var doc = _db.Documents.FirstOrDefault(d => d.DocumentId == documentId);
            if (doc == null)
                return NotFound("Document not found.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", doc.FilePath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File does not exist on the server.");

            return PhysicalFile(filePath, "application/octet-stream", doc.FileName);
        }

    }
}