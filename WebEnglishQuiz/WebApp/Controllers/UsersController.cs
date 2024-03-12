using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	public class UsersController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
