using System.Diagnostics;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using CMCS_PROG6212_POE.Data;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var totalClaims = DataStore.Claims.Count;
        var unverified = DataStore.Claims.Where(c => c.Status == "Pending").Count();
        var verified = DataStore.Claims.Where(c => c.Status == "Verified").Count();
        var approved = DataStore.Claims.Where(c => c.Status == "Approved").Count();
        var rejected = DataStore.Claims.Where(c => c.Status == "Rejected").Count();

        ViewData["TotalClaims"] = totalClaims;
        ViewData["Unverified"] = unverified;
        ViewData["Verified"] = verified;
        ViewData["Approved"] = approved;
        ViewData["Rejected"] = rejected;

        return View();
    }
}
