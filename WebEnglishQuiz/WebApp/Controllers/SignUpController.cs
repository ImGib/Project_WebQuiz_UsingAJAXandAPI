using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class SignUpController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
