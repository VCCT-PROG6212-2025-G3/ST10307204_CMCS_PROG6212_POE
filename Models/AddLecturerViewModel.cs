// Models/ViewModels/AddLecturerViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models.ViewModels
{
    public class AddLecturerViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 9999.99)]
        [Display(Name = "Hourly Rate (R)")]
        public decimal HourlyRate { get; set; }
    }
}