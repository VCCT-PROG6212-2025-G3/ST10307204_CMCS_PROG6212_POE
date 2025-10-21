using CMCS_PROG6212_POE.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly IDataStore _dataStore;

    public HomeController(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IActionResult Index()
    {
        var totalClaims = _dataStore.Claims.Count;
        var unverified = _dataStore.Claims.Where(c => c.Status == "Pending").Count();
        var verified = _dataStore.Claims.Where(c => c.Status == "Verified").Count();
        var approved = _dataStore.Claims.Where(c => c.Status == "Approved").Count();
        var rejected = _dataStore.Claims.Where(c => c.Status == "Rejected").Count();

        ViewData["TotalClaims"] = totalClaims;
        ViewData["Unverified"] = unverified;
        ViewData["Verified"] = verified;
        ViewData["Approved"] = approved;
        ViewData["Rejected"] = rejected;

        return View();
    }
}