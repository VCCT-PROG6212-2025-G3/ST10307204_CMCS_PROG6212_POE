using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class LecturerModel
    {
        public int LecturerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Range(0, 9999.99)]
        public decimal HourlyRate { get; set; }
    }
}
