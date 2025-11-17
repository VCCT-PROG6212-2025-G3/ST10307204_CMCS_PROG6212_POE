// Controllers/LecturerController.cs
using System.Security.Cryptography;
using System.Text;
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly AppDbContext _db;
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public LecturerController(AppDbContext db)
        {
            _db = db;
        }

        // Dashboard – Show only current lecturer's claims
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var claims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Where(c => c.UserId == userId.Value)
                .ToList();

            return View(claims);
        }

        // Submit Claim Form
        public IActionResult SubmitClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var lecturer = _db.Lecturers.FirstOrDefault(l => l.UserId == userId.Value);
            if (lecturer == null)
            {
                TempData["Error"] = "You are not registered as a Lecturer.";
                return RedirectToAction("Index");
            }

            ViewBag.HourlyRate = lecturer.HourlyRate;
            return View(new ClaimModel { HourlyRate = lecturer.HourlyRate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(ClaimModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var lecturer = _db.Lecturers.FirstOrDefault(l => l.UserId == userId.Value);
            if (lecturer == null)
            {
                TempData["Error"] = "Only Lecturers can submit claims.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                model.UserId = userId.Value;
                model.Status = "Pending";
                model.SubmittedDate = DateTime.Now;
                model.Approval = new ApprovalModel();

                _db.Claims.Add(model);
                _db.SaveChanges();

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.HourlyRate = lecturer.HourlyRate;
            return View(model);
        }

        // Upload supporting documents
        public IActionResult UploadDocuments(int? claimId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var claims = _db.Claims
                .Where(c => c.UserId == userId.Value && c.Status == "Pending")
                .ToList();

            ViewBag.SelectedClaimId = claimId;
            return View(claims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadDocuments(int claimId, List<IFormFile> files)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var claim = _db.Claims
                .Include(c => c.Documents)
                .FirstOrDefault(c => c.ClaimId == claimId && c.UserId == userId.Value && c.Status == "Pending");

            if (claim == null)
            {
                TempData["Error"] = "Claim not found or not pending.";
                return RedirectToAction("UploadDocuments");
            }

            // Size check (10MB total per claim)
            var currentSize = claim.Documents.Sum(d => d.FileSize);
            var newSize = files.Sum(f => f.Length);
            if (currentSize + newSize > 10 * 1024 * 1024)
            {
                TempData["Error"] = "Total upload size exceeds 10MB limit.";
                return RedirectToAction("UploadDocuments", new { claimId });
            }

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);

            foreach (var file in files.Where(f => f.Length > 0))
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!new[] { ".pdf", ".docx", ".xlsx" }.Contains(ext))
                {
                    TempData["Error"] = $"File {file.FileName} has invalid type.";
                    return RedirectToAction("UploadDocuments", new { claimId });
                }

                using var ms = new MemoryStream();
                file.CopyTo(ms);
                var encrypted = EncryptFile(ms.ToArray());
                var encryptedName = $"enc_{Guid.NewGuid()}{ext}";
                var path = Path.Combine(_uploadPath, encryptedName);
                System.IO.File.WriteAllBytes(path, encrypted);

                claim.Documents.Add(new DocumentModel
                {
                    FileName = file.FileName,
                    FilePath = encryptedName,
                    FileSize = file.Length,
                    ClaimId = claimId
                });
            }

            _db.SaveChanges();
            TempData["Success"] = "Documents uploaded successfully!";
            return RedirectToAction("UploadDocuments", new { claimId });
        }

        private byte[] EncryptFile(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes("16-char-key-1234"); // In prod: use secure key
            aes.IV = new byte[16]; // In prod: generate random IV
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}