using System.IO;
using System.Security.Cryptography;
using System.Text;
using CMCS_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;
using CMCS_PROG6212_POE.Helpers;
namespace CMCS_PROG6212_POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

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
                switch (action)
                {
                    case "Approve":
                        claim.Status = "Approved";
                        TempData["SuccessMessage"] = $"Claim #{claimId} has been approved.";
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
            return View(DataStore.Claims.Where(c => c.Status == "Verified").ToList());
        }

        public IActionResult ViewClaim()
        {
            var claimId = TempData["SelectedClaimId"] as int?;
            if (!claimId.HasValue)
            {
                TempData["ErrorMessage"] = "No claim selected for viewing.";
                return RedirectToAction("Index");
            }

            var claim = DataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Verified");
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found or not verified.";
                return RedirectToAction("Index");
            }
            return View(claim);
        }

        [HttpGet]
        public IActionResult GetDocument(int claimId, string fileName)
        {
            var claim = DataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
            {
                return NotFound("Claim not found.");
            }

            var document = claim.Documents.FirstOrDefault(d => d.FileName == fileName);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            var filePath = Path.Combine(_uploadPath, document.FilePath);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server.");
            }

            var encryptedBytes = System.IO.File.ReadAllBytes(filePath);
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("16-char-key-1234");
                aes.IV = new byte[16];
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                    cs.Close();
                    var decryptedBytes = ms.ToArray();
                    return File(decryptedBytes, MimeTypes.GetMimeType(fileName), fileName);
                }
            }
        }
    }
}