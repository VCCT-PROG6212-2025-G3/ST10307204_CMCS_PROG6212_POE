// Controllers/HRController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Filters;
using CMCS_PROG6212_POE.Models;
using CMCS_PROG6212_POE.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;


namespace CMCS_PROG6212_POE.Controllers
{
    // Controllers/HRController.cs
    [AuthorizeRole("HR")]
    public class HRController : Controller
    {
        private readonly AppDbContext _db;

        public HRController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            ViewBag.TotalLecturers = _db.Lecturers.Count();
            ViewBag.TotalClaims = _db.Claims.Count();
            ViewBag.PendingClaims = _db.Claims.Count(c => c.Status == "Pending");
            ViewBag.ApprovedClaims = _db.Claims.Count(c => c.Status == "Approved");

            var recentClaims = _db.Claims
                .Include(c => c.User)
                .OrderByDescending(c => c.SubmittedDate)
                .Take(5)
                .ToList();

            return View(recentClaims);
        }

        public IActionResult Lecturers()
        {
            var lecturers = _db.Lecturers
                .Include(l => l.User)
                .ToList();
            return View(lecturers);
        }

        public IActionResult AddLecturer() => View(new AddLecturerViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(AddLecturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                var hasher = new PasswordHasher<User>();

                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PasswordHash = hasher.HashPassword(null!, model.Password),
                    Role = UserRole.Lecturer
                };

                _db.Users.Add(newUser);
                _db.SaveChanges();

                var lecturer = new LecturerModel
                {
                    UserId = newUser.UserId,
                    HourlyRate = model.HourlyRate
                };
                _db.Lecturers.Add(lecturer);
                _db.SaveChanges();

                TempData["Success"] = $"Lecturer {model.FirstName} {model.LastName} added successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult LecturerDetails(int id)
        {
            var lecturer = _db.Lecturers
                .Include(l => l.User)
                .FirstOrDefault(l => l.UserId == id);

            if (lecturer == null) return NotFound();

            return View(lecturer);
        }

        [HttpGet]
        public IActionResult EditLecturer(int id)
        {
            var lecturer = _db.Lecturers.Include(l => l.User).FirstOrDefault(l => l.UserId == id);
            if (lecturer == null) return NotFound();

            var model = new AddLecturerViewModel
            {
                FirstName = lecturer.User.FirstName,
                LastName = lecturer.User.LastName,
                Email = lecturer.User.Email,
                HourlyRate = lecturer.HourlyRate
            };

            ViewBag.UserId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLecturer(int id, AddLecturerViewModel model)
        {
            if (id <= 0) return NotFound();

            var lecturer = _db.Lecturers
                .Include(l => l.User)
                .FirstOrDefault(l => l.UserId == id);
            if (lecturer == null) return NotFound();

            if (ModelState.IsValid)
            {
                lecturer.User.FirstName = model.FirstName;
                lecturer.User.LastName = model.LastName;
                lecturer.User.Email = model.Email;
                lecturer.HourlyRate = model.HourlyRate;

                // Only update password if the field is not empty
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var hasher = new PasswordHasher<User>();
                    lecturer.User.PasswordHash = hasher.HashPassword(lecturer.User, model.Password);
                }

                _db.SaveChanges();

                TempData["Success"] = "Lecturer updated successfully!";
                return RedirectToAction("Lecturers");
            }

            ViewBag.UserId = id;
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteLecturer(int id)
        {
            var lecturer = _db.Lecturers.Include(l => l.User).FirstOrDefault(l => l.UserId == id);
            if (lecturer != null)
            {
                _db.Users.Remove(lecturer.User); // Deletes both User + Lecturer (cascade)
                _db.SaveChanges();
                TempData["Success"] = "Lecturer deleted successfully.";
            }
            return RedirectToAction("Lecturers");
        }
        public IActionResult AllClaims()
        {
            var claims = _db.Claims
                .Include(c => c.User)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }
        public IActionResult ClaimDetails(int id)
        {
            var claim = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Include(c => c.Approval)
                    .ThenInclude(a => a!.VerifiedBy)
                .Include(c => c.Approval)
                    .ThenInclude(a => a!.ApprovedBy)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null) return NotFound();

            return View(claim);
        }

        public IActionResult PaymentReport()
        {
            var approvedClaims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Approval)
                .Where(c => c.Status == "Approved")
                .OrderBy(c => c.User.LastName)
                .ThenBy(c => c.User.FirstName)
                .ToList();

            ViewBag.TotalAmount = approvedClaims.Sum(c => c.TotalAmount);
            ViewBag.ClaimCount = approvedClaims.Count;
            ViewBag.GeneratedOn = DateTime.Now;

            return View(approvedClaims);
        }

 

public IActionResult DownloadClaimInvoice(int claimId)
    {
        var claim = _db.Claims
            .Include(c => c.User)
            .Include(c => c.Documents)
            .Include(c => c.Approval)
                .ThenInclude(a => a!.VerifiedBy)
            .Include(c => c.Approval)
                .ThenInclude(a => a!.ApprovedBy)
            .FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Approved");

        if (claim == null)
        {
            TempData["Error"] = "Approved claim not found.";
            return RedirectToAction("AllClaims");
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                page.Header()
                    .AlignCenter()
                    .PaddingBottom(20)
                    .Text("CLAIM PAYMENT INVOICE")
                    .FontSize(28)
                    .Bold()
                    .FontColor("#1e40af");

                page.Content()
                    .Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text("IIE Varsity College Cape Town").FontSize(14).Bold();
                                left.Item().Text("Finance & Payroll Department");
                                left.Item().Text("151 CampGround Road, Newlands 7708");
                                left.Item().Text("Email: payroll@vcconnect.edu.za");
                            });

                            row.ConstantItem(180).AlignRight().Column(right =>
                            {
                                right.Item().AlignRight().Text($"Invoice No: CMCS-{claim.ClaimId:D6}")
                                    .FontSize(12).Bold();
                                right.Item().AlignRight().Text($"Issue Date: {DateTime.Now:yyyy-MM-dd}");
                                right.Item().AlignRight().Text($"Claim Date: {claim.SubmittedDate:yyyy-MM-dd}");
                                right.Item().AlignRight().PaddingTop(10)
                                    .Text("STATUS: APPROVED")
                                    .FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                            });
                        });

                        col.Item().PaddingTop(30).LineHorizontal(2).LineColor(Colors.Grey.Lighten1);

                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().Column(payee =>
                            {
                                payee.Item().Text("PAY TO:").Bold().FontSize(12);
                                payee.Item().Text($"{claim.User.FirstName} {claim.User.LastName}")
                                    .FontSize(16).Bold();
                                payee.Item().Text(claim.User.Email);
                                payee.Item().Text("Lecturer");
                            });

                            row.ConstantItem(200).AlignRight().Column(amount =>
                            {
                                amount.Item().Text("PAYMENT SUMMARY").Bold();
                                amount.Item().PaddingTop(10).Text($"Hours Worked: {claim.HoursWorked:F1}");
                                amount.Item().Text($"Hourly Rate: R {claim.HourlyRate:N2}");
                                amount.Item().PaddingTop(15).Text("TOTAL PAYABLE:")
                                    .FontSize(16).Bold();
                                amount.Item().Text($"R {claim.TotalAmount:N2}")
                                    .FontSize(32).Bold().FontColor(Colors.Green.Darken3);
                            });
                        });

                        col.Item().PaddingTop(40).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Text("Notes:").Bold();
                            table.Cell().Text(string.IsNullOrEmpty(claim.Notes) ? "No additional notes." : claim.Notes);

                            table.Cell().Text("Submitted On:").Bold();
                            table.Cell().Text(claim.SubmittedDate.ToString("dddd, dd MMMM yyyy 'at' HH:mm"));

                            if (claim.Approval != null)
                            {
                                var verifiedBy = claim.Approval.VerifiedBy != null
                                    ? $"{claim.Approval.VerifiedBy.FirstName} {claim.Approval.VerifiedBy.LastName}"
                                    : "Not Verified";

                                var approvedBy = claim.Approval.ApprovedBy != null
                                    ? $"{claim.Approval.ApprovedBy.FirstName} {claim.Approval.ApprovedBy.LastName}"
                                    : "Not Approved";

                                table.Cell().Text("Verified By:").Bold();
                                table.Cell().Text(verifiedBy + " (coordinator)");

                                table.Cell().Text("Approved By:").Bold();
                                table.Cell().Text(approvedBy + " (Manager)");
                            }
                            else
                            {
                                table.Cell().Text("Status:").Bold();
                                table.Cell().Text("Approved");
                            }
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .PaddingTop(20)
                    .Text("Thank you for your valuable contribution to CMCU.")
                    .FontSize(10)
                    .Italic()
                    .FontColor(Colors.White);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf",
            $"CMCU_Claim_Invoice_{claim.ClaimId}_{claim.User.LastName}_{claim.User.FirstName}.pdf");
    }
}
}
