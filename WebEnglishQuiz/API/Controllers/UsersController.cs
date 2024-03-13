using API.Common;
using API.Common.DTOs.UserDTO;
using API.JWTAuth;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Common.Utility;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDisplay>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            List<User> rs = await _context.Users.ToListAsync();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<UserDisplay>>(rs)));
        }

        [HttpGet("Dashboard/User")]
        public async Task<ActionResult> DashboardNumberUser()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            //get Users
            int numberUser = _context.Users.Count();

            //Get Top 5 Enroll users with the most subjects
            var result = _context.Users.Include(p => p.Subjectnos)
                            .OrderByDescending(t => t.Subjectnos.Count())
                            .Take(5).ToList();

            return Ok(new ResponseStatus(message: ResponseOk, data: new
            {
                Users = numberUser,
                Top = _mapper.Map<List<UserDisplay>>(result)
            }));
        }
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
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
        }
        [HttpGet("Profile/{username}")]
        public async Task<ActionResult<UserProfile>> Profile(string username)
        {
            UserProfile userProfile = new UserProfile();
            try
            {
                if (username == null)
                {
                    throw new Exception();
                }

                username = username.ToUpper().Trim();

                var user = await _context.Users.Include(p => p.Subjectnos).SingleOrDefaultAsync(p => p.Username.ToUpper().Trim().Equals(username));

                if (user == null)
                {
                    throw new Exception();
                }

                //set profile
                userProfile.Username = username;
                userProfile.Email = user.Email;
                userProfile.FirstName = user.FirstName;
                userProfile.LastName = user.LastName;
                userProfile.Phonenumber = user.Phonenumber;
                userProfile.EnrollNum = user.Subjectnos.Count;

                //get most tested subject 
                List<MostSubjectTest> subquery = _context.Histories
                    .GroupBy(t => new { t.Username, t.Subjectno })
                    .Select(g => new MostSubjectTest
                    {
                        username = g.Key.Username,
                        subject = g.Key.Subjectno,
                        times = g.Select(t => t.Testno).Distinct().Count()
                    }).ToList();

                var result = subquery
                    .GroupBy(t => t.username)
                    .Select(g => new
                    {
                        Group = g,
                        MaxTestCount = g.Max(t => t.times)
                    })
                    .SelectMany(g => g.Group.Where(t => t.times == g.MaxTestCount))
                    .ToList();

                var fn = result.SingleOrDefault(p => p.username.ToUpper().Trim().Equals(username));

                if (fn == null)
                {
                    userProfile.MostSubject = "HAVE NOT ENROLL QUIZ";
                }
                else
                {
                    var sub = await _context.Subjects.FindAsync(fn.subject);

                    userProfile.MostSubject = sub.Title;
                }
            }
            catch
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, userProfile));
        }

        [HttpGet("CheckEnroll/{Username}/{Subjectno}")]
        public async Task<ActionResult<UserDisplay>> CheckEnroll(string Username, int Subjectno)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            Username = Username.ToUpper().Trim();

            User? user = await _context.Users.Include(p => p.Subjectnos).SingleOrDefaultAsync(p => p.Username.ToUpper().Trim().Equals(Username));

            if (user == null)
            {
                return Ok(new ResponseStatus(RequestEnroll));
            }

            foreach (Subject sub in user.Subjectnos)
            {
                if (sub.Subjectno == Subjectno)
                    return Ok(new ResponseStatus(ResponseOk));
            }

            return Ok(new ResponseStatus(RequestEnroll));
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDisplay>> Login(UserLogin data)
        {
            User? user = await _context.Users.SingleOrDefaultAsync(p => p.Username.ToLower().Trim().Equals(data.Username.ToLower().Trim())
                                                                    && p.Password.ToLower().Trim().Equals(data.Password.ToLower().Trim())
                                                                    && p.Status == true); //Account enable
            if (user == null)
            {
                return Ok(new ResponseStatus(message: LoginFail));
            }

            JwtResponse token = CreateToken(_config, user);
            return Ok(new ResponseStatus(message: LoginOk, token));
        }
        [HttpPut("changepassword")]
        public async Task<IActionResult> Changepassword(UserChangepassword data)
        {
            if (!UserExists(data.UserName))
            {
                return NotFound(new ResponseStatus(message: UserNotExisted));
            }

            if (!ConfirmPassword(data.Password, data.ConfirmPassword))
            {
                return NotFound(new ResponseStatus(message: ConfirmPassFail));
            }

            User? user = await _context.Users.FindAsync(data.UserName);

            user.Password = data.ConfirmPassword;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
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
                return Ok(new ResponseStatus(message: EmailExisted, data: _mapper.Map<UserDisplay>(ur)));
            }
            if (PhoneNumberExits(user.Phonenumber))
            {
                return Ok(new ResponseStatus(message: PhoneNumberExisted, data: _mapper.Map<UserDisplay>(ur)));
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
                    return Conflict(new ResponseStatus(message: UserExisted, data: _mapper.Map<UserDisplay>(ur)));
                }
                else
                {
                    return Problem(ResgisterFail);
                }
            }

            return Ok(new ResponseStatus(message: ResgisterOk, data: _mapper.Map<UserDisplay>(ur)));
        }
        [HttpPut("adminUpdate")]
        public async Task<ActionResult> AdminUpUser(UserUpdateBaseBase data)
        {
            if (_context.Users == null)
            {
                return Problem(ResgisterFail);
            }

            User? user = await _context.Users.FindAsync(data.Username);

            if (user == null)
            {
                return Ok(new ResponseStatus(message: "Update User Fail", data: _mapper.Map<UserDisplay>(data)));
            }

            user.Status = data.Status;
            user.Role = data.Role;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Ok(new ResponseStatus(message: ResponseError, data: _mapper.Map<UserDisplay>(data)));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<UserDisplay>(user)));
        }
        [HttpPut("Enroll/{username}/{subjectno}")]
        public async Task<ActionResult> Enroll(string username, int subjectno)
        {
            if (_context.Users == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }
            username = username.ToUpper().Trim();
            User? user = await _context.Users.Include(p => p.Subjectnos).SingleOrDefaultAsync(p => p.Username.ToUpper().Trim().Equals(username));
            if (user == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }
            Subject? sub = await _context.Subjects.FindAsync(subjectno);
            if (sub == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }
            try
            {
                user.Subjectnos.Add(sub);

                await _context.SaveChangesAsync();
            }
            catch
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            return Ok(new ResponseStatus(ResponseOk));
        }

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
