using System.ComponentModel.DataAnnotations;
using CMCS_PROG6212_POE.Models;

public class DocumentModel
{
    [Key]
    public int DocumentId { get; set; }
    public int ClaimId { get; set; }
    public ClaimModel Claim { get; set; }

    [Required]
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; } // Added for size tracking
    public DateTime UploadDate { get; set; } = DateTime.Now;
}