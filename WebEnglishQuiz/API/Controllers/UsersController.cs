using API.Common;
using API.Common.DTOs.UserDTO;
using API.JWTAuth;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Common.Variables;
using static API.JWTAuth.JwtAuthConfig;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly QuizAPIContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UsersController(QuizAPIContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _config = configuration;
        }

        // GET: api/Users
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDisplay>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            List<User> rs = await _context.Users.ToListAsync();
            return Ok(new Utility.ResponseStatus(message: ResponseOk, data: _mapper.Map<List<UserDisplay>>(rs)));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDisplay>> GetUser(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(new Utility.ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDisplay>> Login(UserLogin data)
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.Username.ToLower().Trim().Equals(data.Username.ToLower().Trim())
                                                                    && p.Password.ToLower().Trim().Equals(data.Password.ToLower().Trim())
                                                                    && p.Status == true); //Account enable
            if (user == null)
            {
                return Ok(new Utility.ResponseStatus(message: LoginFail));
            }

            JwtResponse token = CreateToken(_config, user);
            return Ok(new Utility.ResponseStatus(message: LoginOk, token));
        }

        // PUT: api/Users/5
        [HttpPut("changepassword")]
        public async Task<IActionResult> Changepassword(UserChangepassword data)
        {
            if (!UserExists(data.UserName))
            {
                return NotFound(new Utility.ResponseStatus(message: UserNotExisted));
            }

            if (!ConfirmPassword(data.Password, data.ConfirmPassword))
            {
                return NotFound(new Utility.ResponseStatus(message: ConfirmPassFail));
            }

            User? user = await _context.Users.FindAsync(data.UserName);

            user.Password = data.ConfirmPassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Utility.ResponseStatus(message: ResponseError));
            }

            return Ok(new Utility.ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDisplay>> PostUser(UserRegister user)
        {
            if (_context.Users == null)
            {
                return Problem(ResgisterFail);
            }

            User ur = _mapper.Map<User>(user);

            if (EmailExits(user.Email))
            {
                return Ok(new Utility.ResponseStatus(message: EmailExisted, data: _mapper.Map<UserDisplay>(ur)));
            }
            if (PhoneNumberExits(user.Phonenumber))
            {
                return Ok(new Utility.ResponseStatus(message: PhoneNumberExisted, data: _mapper.Map<UserDisplay>(ur)));
            }

            _context.Users.Add(ur);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Username))
                {
                    return Conflict(new Utility.ResponseStatus(message: UserExisted, data: _mapper.Map<UserDisplay>(ur)));
                }
                else
                {
                    return Problem(ResgisterFail);
                }
            }

            return Ok(new Utility.ResponseStatus(message: ResgisterOk, data: _mapper.Map<UserDisplay>(ur)));
        }
        [HttpPut("adminUpdate")]
        public async Task<ActionResult> AdminUpUser(UserUpdateBaseBase data)
        {
            if (_context.Users == null)
            {
                return Problem(ResgisterFail);
            }

            User? user = await _context.Users.FindAsync(data.Username);

            if(user == null)
            {
                return Ok(new Utility.ResponseStatus(message: "Update User Fail", data: _mapper.Map<UserDisplay>(data)));
            }

            user.Status = data.Status;
            user.Role = data.Role;
            try
            {
                await _context.SaveChangesAsync();
            } catch
            {
                return Ok(new Utility.ResponseStatus(message: ResponseError, data: _mapper.Map<UserDisplay>(data)));
            }

            return Ok(new Utility.ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
        }

        //[HttpGet("Logout/{username}")]
        //public async Task<ActionResult<UserDisplay>> Logout(string username)
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return Ok(new Utility.ResponseStatus(message: ResponseOk));
        //}
        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        ////[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteUser(string id)
        //{
        //    if (_context.Users == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool UserExists(string id)
        {
            return (_context.Users?.Any(e => e.Username == id)).GetValueOrDefault();
        }
        private bool EmailExits(string? email)
        {
            if (email == null) return false;
            return (_context.Users?.Any(e => e.Email.ToUpper().Trim().Equals(email.ToUpper().Trim()))).GetValueOrDefault();
        }
        private bool PhoneNumberExits(string? phone)
        {
            if (phone == null) return false;
            return (_context.Users?.Any(e => e.Phonenumber.ToUpper().Trim().Equals(phone.ToUpper().Trim()))).GetValueOrDefault();
        }
        private bool ConfirmPassword(string oldpass, string newpass)
        {
            return oldpass.ToLower().Trim().Equals(newpass.ToLower().Trim());
        }
    }
}
