using System.ComponentModel.DataAnnotations;

namespace CMCS_PROG6212_POE.Models
{
    public class DocumentModel
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }
        [Required]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
