// Controllers/AccountController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Email and password are required.";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Invalid email or password.";
                return View();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                TempData["Error"] = "Invalid email or password.";
                return View();
            }

            // Set Session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

            // Redirect by Role
            return user.Role switch
            {
                UserRole.HR => RedirectToAction("Dashboard", "HR"),
                UserRole.Lecturer => RedirectToAction("Dashboard", "Lecturer"),
                UserRole.Coordinator => RedirectToAction("Dashboard", "Coordinator"),
                UserRole.Manager => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Login")
            };
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}