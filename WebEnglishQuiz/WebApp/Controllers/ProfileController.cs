using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            string? username = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            ViewData["Username"] = username;
            return View();
        }
    }
}
