using System.Diagnostics;
using CMCS_PROG6212_POE.Models;
using Microsoft.AspNetCore.Mvc;

namespace CMCS_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
