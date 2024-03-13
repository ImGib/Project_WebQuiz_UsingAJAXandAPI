using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class SubjectsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateSubject()
        {
            return View();
        }
        public IActionResult UpdateSubject()
        {
            return View();
        }
		public IActionResult SubjectCard()
		{
			return View();
		}
	}
}
