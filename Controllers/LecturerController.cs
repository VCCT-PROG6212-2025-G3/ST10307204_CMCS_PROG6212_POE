using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using CMCS_PROG6212_POE.Data;

namespace CMCS_PROG6212_POE.Controllers
{

    public class LecturerController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult SubmitClaim() => View(new ClaimModel());
        [HttpPost]
        public IActionResult SubmitClaim(ClaimModel model, IFormFile file)
        {
            if (ModelState.IsValid && file != null && file.Length <= 5 * 1024 * 1024 && new[] { ".pdf", ".docx", ".xlsx" }.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                model.ClaimId = DataStore.Claims.Count + 1;
                model.Approval = new ApprovalModel();
                var doc = new DocumentModel { FileName = file.FileName, FilePath = "encrypted/path" }; // Encryption placeholder
                model.Documents.Add(doc);
                DataStore.Claims.Add(model);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Invalid file or form data.");
            return View(model);
        }
        public IActionResult TrackClaims() => View(DataStore.Claims);
    }
}
