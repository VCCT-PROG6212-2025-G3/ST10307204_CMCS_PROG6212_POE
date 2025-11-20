using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Filters;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    [AuthorizeRole("Lecturer")]
    public class LecturerController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        public LecturerController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Lecturer Dashboard
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var claims = _db.Claims
                .Include(c => c.Documents)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }

        // Show claim submission form
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            var lecturer = _db.Lecturers.FirstOrDefault(l => l.UserId == userId.Value);
            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer record not found.";
                return RedirectToAction("Index");
            }

            var model = new ClaimModel
            {
                HourlyRate = lecturer.HourlyRate,
                HoursWorked = 0,
                Notes = string.Empty
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(ClaimModel model, List<IFormFile> files)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index");
            }

            var lecturer = _db.Lecturers.FirstOrDefault(l => l.UserId == userId.Value);
            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer record not found.";
                return RedirectToAction("Index");
            }

            // === NEW: HOURS VALIDATION (MAX 180 PER MONTH) ===
            if (model.HoursWorked > 180)
            {
                ModelState.AddModelError("HoursWorked",
                    "You cannot claim more than 180 hours in a single month. Please adjust your hours.");
            }

            // Optional: Also prevent negative or zero hours
            if (model.HoursWorked <= 0)
            {
                ModelState.AddModelError("HoursWorked",
                    "Hours worked must be greater than zero.");
            }

            // File validation
            if (files == null || !files.Any() || files.All(f => f.Length == 0))
            {
                ModelState.AddModelError("", "You must upload at least one supporting document.");
            }

            // If validation fails — return to form with errors
            if (!ModelState.IsValid)
            {
                // Re-populate the form with lecturer's rate
                model.HourlyRate = lecturer.HourlyRate;
                return View(model);
            }

            try
            {
                model.UserId = userId.Value;
                model.HourlyRate = lecturer.HourlyRate;
                model.Status = "Pending";
                model.SubmittedDate = DateTime.Now;
                model.Approval = new ApprovalModel();
                model.Documents = new List<DocumentModel>();

                if (!Directory.Exists(_uploadPath))
                    Directory.CreateDirectory(_uploadPath);

                foreach (var file in files.Where(f => f.Length > 0))
                {
                    var ext = Path.GetExtension(file.FileName).ToLower();
                    if (!new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".jpeg", ".png" }.Contains(ext))
                    {
                        ModelState.AddModelError("", $"Invalid file type: {file.FileName}");
                        model.HourlyRate = lecturer.HourlyRate;
                        return View(model);
                    }

                    var fileName = $"doc_{Guid.NewGuid()}{ext}";
                    var path = Path.Combine(_uploadPath, fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    model.Documents.Add(new DocumentModel
                    {
                        FileName = file.FileName,
                        FilePath = fileName,
                        FileSize = file.Length
                    });
                }

                _db.Claims.Add(model);
                _db.SaveChanges();

                TempData["Success"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while submitting your claim.");
                model.HourlyRate = lecturer.HourlyRate;
                return View(model);
            }
        }
        // TrackClaim
        public IActionResult TrackClaim()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Index", "Home");
            }

            var claims = _db.Claims
                .Include(c => c.Documents) // Include documents
                .Include(c => c.Approval)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }

    }
}
