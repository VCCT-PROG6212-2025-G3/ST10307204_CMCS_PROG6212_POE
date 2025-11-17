// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CMCS_PROG6212_POE.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Store hashed password

        [Required]
        public UserRole Role { get; set; }

        public decimal? HourlyRate { get; set; }  // Only for Lecturers

        // Navigation
        public ICollection<ClaimModel> Claims { get; set; } = new List<ClaimModel>();
    }

    public enum UserRole
    {
        HR,
        Lecturer,
        Coordinator,
        Manager
    }
}