using System.IO;
using System.Security.Cryptography;
using System.Text;
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        private readonly AppDbContext _db;

        public CoordinatorController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var pendingClaims = _db.Claims.Where(c => c.Status == "Pending").ToList();
            return View(pendingClaims);
        }

        //[HttpPost]
        //public IActionResult Index(int claimId, string action)
        //{
        //    var claim = _db.Claims.FirstOrDefault(c => c.ClaimId == claimId);
        //    if (claim != null)
        //    {
        //        claim.Approval.CoordinatorId = 1; // Mock ID
        //        switch (action)
        //        {
        //            case "Verify":
        //                claim.Status = "Verified";
        //                TempData["SuccessMessage"] = $"Claim #{claimId} has been verified and sent to the Manager.";
        //                break;
        //            case "Reject":
        //                claim.Status = "Rejected";
        //                TempData["SuccessMessage"] = $"Claim #{claimId} has been rejected.";
        //                break;
        //            case "View":
        //                TempData["SelectedClaimId"] = claimId;
        //                return RedirectToAction("ViewClaim");
        //            default:
        //                TempData["ErrorMessage"] = "Invalid action selected.";
        //                break;
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    TempData["ErrorMessage"] = "Claim not found.";
        //    return View(_db.Claims.Where(c => c.Status == "Pending").ToList());
        //}

        //public IActionResult ViewClaim()
        //{
        //    var claimId = TempData["SelectedClaimId"] as int?;
        //    if (!claimId.HasValue)
        //    {
        //        TempData["ErrorMessage"] = "No claim selected for viewing.";
        //        return RedirectToAction("Index");
        //    }

        //    var claim = _db.Claims.FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");
        //    if (claim == null)
        //    {
        //        TempData["ErrorMessage"] = "Claim not found or not pending.";
        //        return RedirectToAction("Index");
        //    }
        //    return View(claim);
        //}

        //[HttpGet]
        //public IActionResult GetDocument(int claimId, string fileName)
        //{
        //    var claim = _db.Claims.FirstOrDefault(c => c.ClaimId == claimId);
        //    if (claim == null)
        //    {
        //        return NotFound("Claim not found.");
        //    }

        //    var document = claim.Documents.FirstOrDefault(d => d.FileName == fileName);
        //    if (document == null)
        //    {
        //        return NotFound("Document not found.");
        //    }

        //    var filePath = Path.Combine(_uploadPath, document.FilePath);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound("File not found on server.");
        //    }

        //    var encryptedBytes = System.IO.File.ReadAllBytes(filePath);
        //    using (var aes = Aes.Create())
        //    {
        //        aes.Key = Encoding.UTF8.GetBytes("16-char-key-1234");
        //        aes.IV = new byte[16];
        //        using (var ms = new MemoryStream())
        //        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
        //        {
        //            cs.Write(encryptedBytes, 0, encryptedBytes.Length);
        //            cs.Close();
        //            var decryptedBytes = ms.ToArray();
        //            return File(decryptedBytes, MimeTypes.GetMimeType(fileName), fileName);
        //        }
        //    }
        //}
    }
}