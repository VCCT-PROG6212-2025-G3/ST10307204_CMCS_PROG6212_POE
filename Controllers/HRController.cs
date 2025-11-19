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

    }
}