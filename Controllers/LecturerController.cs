using System.IO;
using System.Security.Cryptography;
using System.Text;
using CMCS_PROG6212_POE.Data;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
 using System.Linq;
using CMCS_PROG6212_POE.Interfaces;

namespace CMCS_PROG6212_POE.Controllers
{
    public class LecturerController : Controller
    {
        private readonly IDataStore _dataStore;
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public LecturerController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public IActionResult Index() => View();
        public IActionResult SubmitClaim() => View(new ClaimModel());
        [HttpPost]
        public IActionResult SubmitClaim(ClaimModel model)
        {
            model.Approval ??= new ApprovalModel();

            if (ModelState.IsValid)
            {
                _dataStore.AddClaim(model);
                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Error submitting claim: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(model);
        }
        public IActionResult TrackClaims() => View(_dataStore.Claims);
        public IActionResult UploadDocuments()
        {
            var pendingClaims = _dataStore.Claims.Where(c => c.Status == "Pending").ToList();
            TempData["SelectedClaimId"] = null;
            return View(pendingClaims);
        }
        [HttpPost]
        public IActionResult UploadDocuments(int claimId, List<IFormFile> files)
        {
            if (claimId == -1)
            {
                TempData.Remove("SuccessMessage");
                TempData.Remove("ErrorMessage");
                TempData["SelectedClaimId"] = null;
                return RedirectToAction("UploadDocuments");
            }

            var claim = _dataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Invalid or non-pending claim.";
                TempData["SelectedClaimId"] = null;
                return RedirectToAction("UploadDocuments");
            }

            var currentSize = claim.Documents.Sum(d => d.FileSize);
            var totalNewSize = files?.Sum(f => f?.Length ?? 0) ?? 0;

            if (currentSize + totalNewSize > 10 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "Total file size for this claim exceeds 10 MB.";
                TempData["SelectedClaimId"] = claimId;
                return RedirectToAction("UploadDocuments");
            }

            if (files != null && files.Any())
            {
                if (!Directory.Exists(_uploadPath))
                {
                    Directory.CreateDirectory(_uploadPath);
                }

                foreach (var file in files.Where(f => f != null && f.Length > 0))
                {
                    try
                    {
                        if (new[] { ".pdf", ".docx", ".xlsx" }.Contains(Path.GetExtension(file.FileName).ToLower()))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                file.CopyTo(memoryStream);
                                var fileBytes = memoryStream.ToArray();
                                var encryptedBytes = EncryptFile(fileBytes);
                                var encryptedFileName = $"encrypted_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                                var filePath = Path.Combine(_uploadPath, encryptedFileName);
                                System.IO.File.WriteAllBytes(filePath, encryptedBytes);
                                claim.Documents.Add(new DocumentModel
                                {
                                    FileName = file.FileName,
                                    FilePath = encryptedFileName,
                                    FileSize = file.Length,
                                    ClaimId = claimId
                                });
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = $"Invalid file type for {file.FileName}.";
                            TempData["SelectedClaimId"] = claimId;
                            return RedirectToAction("UploadDocuments");
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = $"Error uploading {file.FileName}: {ex.Message}";
                        TempData["SelectedClaimId"] = claimId;
                        return RedirectToAction("UploadDocuments");
                    }
                }
                TempData["SuccessMessage"] = $"Successfully uploaded {files.Count} document(s) to Claim #{claimId}.";
                TempData["SelectedClaimId"] = claimId;
                return RedirectToAction("UploadDocuments");
            }
            else
            {
                TempData["ErrorMessage"] = "No files uploaded.";
                TempData["SelectedClaimId"] = claimId;
                return RedirectToAction("UploadDocuments");
            }
        }

        private byte[] EncryptFile(byte[] fileBytes)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("16-char-key-1234");
                aes.IV = new byte[16];
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(fileBytes, 0, fileBytes.Length);
                    cs.Close();
                    return ms.ToArray();
                }
            }
        }

        [HttpGet]
        public IActionResult GetClaimDocuments(int claimId)
        {
            var claim = _dataStore.Claims.FirstOrDefault(c => c.ClaimId == claimId && c.Status == "Pending");
            if (claim == null)
            {
                return Content("<span class='badge bg-secondary rounded-pill px-3 py-2'>No documents or claim not found</span>");
            }

            if (!claim.Documents.Any())
            {
                return Content("<span class='badge bg-secondary rounded-pill px-3 py-2'>No documents yet</span>");
            }

            var html = new StringBuilder();
            html.Append("<div>");
            foreach (var doc in claim.Documents)
            {
                html.Append($"<span class='badge bg-secondary rounded-pill px-3 py-2 me-2'>{doc.FileName}</span>");
            }
            html.Append($"<p class='text-muted mt-2'>Total Size: {((claim.Documents.Sum(d => d.FileSize) / 1024.0 / 1024.0).ToString("0.##"))} MB</p>");
            html.Append("</div>");
            return Content(html.ToString(), "text/html");
        }
    }
}
