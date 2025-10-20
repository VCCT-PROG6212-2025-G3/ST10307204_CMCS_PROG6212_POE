using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class ManagerModel
    {
        public int ManagerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
