using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class ExamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
