using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_PROG6212_POE.Models
{
    public class LecturerModel
    {
        [Key]
        public int LecturerId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [Range(100, 1000)]
        public decimal HourlyRate { get; set; }
    }
}