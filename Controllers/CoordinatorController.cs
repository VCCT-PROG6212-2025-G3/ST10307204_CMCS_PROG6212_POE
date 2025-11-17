// Controllers/CoordinatorController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly AppDbContext _db;

        public CoordinatorController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var claims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Where(c => c.Status == "Pending")
                .ToList();

            return View(claims);
        }

        [HttpPost]
        public IActionResult Verify(int claimId, string action)
        {
            var claim = _db.Claims
                .Include(c => c.Approval)
                .FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");

            if (claim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction("Index");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (action == "Verify")
            {
                claim.Status = "Verified";
                claim.Approval.VerifiedById = userId;
                claim.Approval.VerifiedDate = DateTime.Now;
                TempData["Success"] = "Claim verified and sent to Manager.";
            }
            else if (action == "Reject")
            {
                claim.Status = "Rejected";
                claim.Approval.VerifiedById = userId;
                claim.Approval.VerifiedDate = DateTime.Now;
                TempData["Success"] = "Claim rejected.";
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}