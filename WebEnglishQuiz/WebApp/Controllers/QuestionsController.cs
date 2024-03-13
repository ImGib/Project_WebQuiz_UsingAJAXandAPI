using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class QuestionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateQuestion()
        {
            return View();
        }
    }
}
