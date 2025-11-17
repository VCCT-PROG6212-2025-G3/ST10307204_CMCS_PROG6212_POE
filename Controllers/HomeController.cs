using CMCS_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var totalClaims = _db.Claims.Count();
        var unverified = _db.Claims.Where(c => c.Status == "Pending").Count();
        var verified = _db.Claims.Where(c => c.Status == "Verified").Count();
        var approved = _db.Claims.Where(c => c.Status == "Approved").Count();
        var rejected = _db.Claims.Where(c => c.Status == "Rejected").Count();

        ViewData["TotalClaims"] = totalClaims;
        ViewData["Unverified"] = unverified;
        ViewData["Verified"] = verified;
        ViewData["Approved"] = approved;
        ViewData["Rejected"] = rejected;

        return View();
    }
}