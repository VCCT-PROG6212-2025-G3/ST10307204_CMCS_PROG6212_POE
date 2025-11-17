// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        // One-to-one relationships
        public LecturerModel? Lecturer { get; set; }
        public CoordinatorModel? Coordinator { get; set; }
        public ManagerModel? Manager { get; set; }
        public HRModel? HR { get; set; }

        public List<ClaimModel> Claims { get; set; } = new List<ClaimModel>();
    }

    public enum UserRole
    {
        HR,
        Lecturer,
        Coordinator,
        Manager
    }
}