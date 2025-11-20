// Controllers/AccountController.cs
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db) => _db = db;

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Please enter email and password.";
                return View();
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "Invalid credentials.";
                return View();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Invalid credentials.";
                return View();
            }

            // Session login
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");

            return user.Role switch
            {
                UserRole.HR => RedirectToAction("Index", "HR"),
                UserRole.Lecturer => RedirectToAction("Index", "Lecturer"),
                UserRole.Coordinator => RedirectToAction("Index", "Coordinator"),
                UserRole.Manager => RedirectToAction("Index", "Manager"),
                _ => RedirectToAction("Login")
            };
        }
        
        public IActionResult AccessDenied()
        {
            // Optional: pass a friendly message
            ViewBag.Message = "You do not have permission to access this page.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}