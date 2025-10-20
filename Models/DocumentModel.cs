using System.ComponentModel.DataAnnotations;

public class DocumentModel
{
    public int DocumentId { get; set; }
    public int ClaimId { get; set; }
    [Required]
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; } // Added for size tracking
    public DateTime UploadDate { get; set; } = DateTime.Now;
}