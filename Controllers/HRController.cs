// Controllers/HRController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Filters;
using CMCS_PROG6212_POE.Models;
using CMCS_PROG6212_POE.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Controllers
{
    [AuthorizeRole("HR")]
    public class HRController : Controller
    {
        private readonly AppDbContext _db;

        public HRController(AppDbContext db)
        {
            _db = db;
        }

        // Dashboard – Show stats + recent activity
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

        // List all users (HR only)
        public IActionResult Users()
        {
            var users = _db.Users
                .Include(u => u.Lecturer)
                .Include(u => u.Coordinator)
                .Include(u => u.Manager)
                .Include(u => u.HR)
                .ToList();

            return View(users);
        }

        // Add new Lecturer (only HR can do this)
        public IActionResult AddLecturer()
        {
            return View(new AddLecturerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLecturer(AddLecturerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
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
                _db.SaveChanges(); // UserId is now generated

                var lecturer = new LecturerModel
                {
                    UserId = newUser.UserId,
                    HourlyRate = model.HourlyRate
                };
                _db.Lecturers.Add(lecturer);
                _db.SaveChanges();

                TempData["Success"] = $"Lecturer {model.FirstName} {model.LastName} added successfully!";
                return RedirectToAction("Users");
            }

            return View(model);
        }

        // View all claims (for reporting)
        public IActionResult AllClaims()
        {
            var claims = _db.Claims
                .Include(c => c.User)
                .Include(c => c.Documents)
                .Include(c => c.Approval)
                .OrderByDescending(c => c.SubmittedDate)
                .ToList();

            return View(claims);
        }
    }
}