using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS_PROG6212_POE.Models
{
    public class ManagerModel
    {
        [Key]
        public int ManagerId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
