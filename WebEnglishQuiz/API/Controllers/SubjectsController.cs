using API.Common.DTOs.SubjectDTO;
using API.Common.DTOs.UserDTO;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Common.Utility;
using static API.Common.Variables;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly QuizAPIContext _context;
        private readonly IMapper _mapper;
        public SubjectsController(QuizAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> GetSubjects()
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }
            List<Subject> list = await _context.Subjects.Include(p => p.CategorynoNavigation).ToListAsync();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<SubjectResponse>>(list)));
        }
        [HttpGet("Dashboard/Subject")]
        public async Task<ActionResult<int>> NumberSubject()
        {
            if (_context.Subjects == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            try
            {

                //Number of subject
                int subjects = _context.Subjects.Count();

                List<SubjectDashboard> subDash = new List<SubjectDashboard>();

                //Get Top 5 subjects with the highest number of enrollees
                List<Subject> listbase = _context.Subjects.Include(p => p.Usernames)
                                        .OrderByDescending(t => t.Usernames.Count())
                                        .Take(5).ToList();

                //list quizzes
                List<UserQuizList> quizListBase = _context.Histories
                                .Select(h => new UserQuizList
                                {
                                    Username = h.Username,
                                    Subjectno = h.Subjectno,
                                    Testno = h.Testno
                                })
                                .Distinct().ToList();

                foreach (Subject subject in listbase)
                {
                    SubjectDashboard subjectDashboard = new SubjectDashboard();
                    subjectDashboard.Name = subject.Title;
                    subjectDashboard.Enroll = subject.Usernames.Count;

                    //get own quizzes
                    List<UserQuizList> ownQuiz = quizListBase.Where(p => p.Subjectno == subject.Subjectno).ToList();
                    //count number quizz that has been created
                    subjectDashboard.QuizNum = ownQuiz.Count();

                    //number of question
                    subjectDashboard.QuestionNum = _context.Questions.Where(p => p.Subjectno == subject.Subjectno).Count();
                    if (subjectDashboard.QuestionNum > 0)
                    {
                        //Passing percentage
                        int percen = 0;
                        //count number of quizz that have pass more than 50%
                        foreach (UserQuizList quizz in ownQuiz)
                        {
                            //get all question for each quiz
                            List<History> histories = _context.Histories
                                .Where(p => p.Username.ToUpper().Trim().Equals(quizz.Username.ToUpper().Trim())
                                && p.Subjectno == quizz.Subjectno
                                && p.Testno == quizz.Testno
                                )
                                .ToList();

                            int number = 0;
                            //count number of correct each quizz
                            foreach (History history in histories)
                            {
                                //check asnwer is  correct or not
                                Answer? ans = await _context.Answers.FindAsync(history.Answerno);
                                if (ans == null) continue;
                                //if ans is correct than plus number of correct
                                if (ans.Iscorect)
                                {
                                    number++;
                                }
                            }
                            //if the number of correct greater than or equal 50% than this quizz is pass
                            if (number / histories.Count * 100 >= 0.5) percen++;
                        }

                        subjectDashboard.PassingPercentage = (int)((double)percen / (double)subjectDashboard.QuestionNum * 100);
                    }
                    else
                    {
                        subjectDashboard.QuestionNum = 0;
                        subjectDashboard.PassingPercentage = 0;
                    }
                    subDash.Add(subjectDashboard);
                }
                return Ok(new ResponseStatus(ResponseOk, new
                {
                    NoSubject = subjects,
                    sublist = subDash
                }));
            }
            catch
            {
                return Ok(new ResponseStatus(ResponseError));
            }
        }
        [HttpGet("Public")]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> PublicSubjects()
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }
            List<Subject> list = _context.Subjects.Include(p => p.CategorynoNavigation).Where(p => p.Status == true).ToList();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<SubjectResponse>>(list)));
        }
        //[HttpGet("GetSubjectNotEndroll/{Username}")]
        //public async Task<ActionResult<SubjectResponseBase>> GetEnrolled(string Username)
        //{
        //    if (_context.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    Username = Username.ToUpper().Trim();

        //    User? user = await _context.Users.Include(p => p.Subjectnos).SingleOrDefaultAsync(p => p.Username.ToUpper().Trim().Equals(Username));

        //    List<Subject>? list = _context.Subjects.Include(p=>p.Usernames).Where(p => p)
        //    if (user == null)
        //    {
        //        return Ok(new ResponseStatus(ResponseError));
        //    }

        //    List<SubjectResponseBase> list = _mapper.Map<List<SubjectResponseBase>>(user.Subjectnos);

        //    return Ok(new ResponseStatus(ResponseOk, list));
        //}
        [HttpGet("Public/{word}")]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> PublicSubjects(string word)
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }
            word = word.ToUpper().Trim();
            List<Subject> list = _context.Subjects.Include(p => p.CategorynoNavigation).Where(p => p.Status == true &&
                                                            (p.Description.ToUpper().Trim().Contains(word)
                                                            || p.Title.ToUpper().Trim().Contains(word)))
                .ToList();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<SubjectResponse>>(list)));
        }
        [HttpGet("Public/Category/{id}")]
        public async Task<ActionResult<IEnumerable<SubjectResponse>>> PublicSubjectsByCategory(int id)
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }
            List<Subject> list = _context.Subjects.Include(p => p.CategorynoNavigation).Where(p => p.Status == true && p.Categoryno == id).ToList();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<SubjectResponse>>(list)));
        }

        // GET: api/Subjects/5
        [HttpGet("{subjectno}")]
        public async Task<ActionResult<Subject>> GetSubject(int subjectno)
        {
            if (_context.Subjects == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            var subject = await _context.Subjects.Include(p => p.CategorynoNavigation).SingleOrDefaultAsync(p => p.Subjectno == subjectno);

            if (subject == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<SubjectResponse>(subject)));
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSubject(int id, SubjectRequest subject)
        {
            Subject? data = await _context.Subjects.Include(p => p.CategorynoNavigation).SingleOrDefaultAsync(p => p.Subjectno == id);

            if (data == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            data.Title = subject.Title;
            data.Description = subject.Description;
            data.Categoryno = subject.Categoryno;
            data.Image = subject.Image;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<SubjectResponse>(data)));
        }
        [HttpPut("updateStatus/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            Subject? data = await _context.Subjects.Include(p => p.CategorynoNavigation).SingleOrDefaultAsync(p => p.Subjectno == id);

            if (data == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            data.Status = !data.Status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<SubjectResponse>(data)));
        }

        // POST: api/Subjects
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> PostSubject(SubjectRequest subject)
        {
            if (_context.Subjects == null)
            {
                return Problem("Entity set 'QuizAPIContext.Subjects'  is null.");
            }
            Subject data = _mapper.Map<Subject>(subject);
            _context.Subjects.Add(data);
            await _context.SaveChangesAsync();

            return Ok(new ResponseStatus(message: ResponseOk, data.Subjectno));
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            if (_context.Subjects == null)
            {
                return NotFound();
            }
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SubjectExists(int id)
        {
            return (_context.Subjects?.Any(e => e.Subjectno == id)).GetValueOrDefault();
        }
    }
}
