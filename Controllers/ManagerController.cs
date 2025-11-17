// Controllers/ManagerController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
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
    }
}