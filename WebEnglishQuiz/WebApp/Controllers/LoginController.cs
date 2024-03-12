using API.Common;
using API.JWTAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace WebApp.Controllers
{
	public class UserLogin
	{
		public string UserName { get; set; }
		public string Password { get; set; }
	}
	public class ResponseUser
	{
		public string Message { get; set; }
		public JwtResponse Data { get; set; }
	}
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Confirm([FromBody] UserLogin user)
		{
			using (HttpClient client = new HttpClient())
			{
				using (HttpResponseMessage response = await client.PostAsJsonAsync("http://localhost:5020/api/Users/login", user))
				{
					using (HttpContent content = response.Content)
					{
						string data = await content.ReadAsStringAsync();
						ResponseUser? res = JsonConvert.DeserializeObject<ResponseUser>(data);
						if (res == null)
						{
							return Ok(new Utility.ResponseStatus(message: "Fail"));
						}
						List<Claim> claims = new List<Claim>()
						{
							new Claim(ClaimTypes.NameIdentifier, res.Data.Username.Trim()),
							new Claim("Role", res.Data.Role)
						};
						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

						var authProperties = new AuthenticationProperties
						{
							AllowRefresh = true,
							IsPersistent = true
						};

						await HttpContext.SignInAsync(
								CookieAuthenticationDefaults.AuthenticationScheme,
								new ClaimsPrincipal(claimsIdentity),
								authProperties
								);
						return Ok(res);
					}
				}
			}
		}
	}
}
