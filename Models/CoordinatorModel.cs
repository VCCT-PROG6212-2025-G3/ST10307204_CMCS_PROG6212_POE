using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class CoordinatorModel
    {
        public int CoordinatorId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
