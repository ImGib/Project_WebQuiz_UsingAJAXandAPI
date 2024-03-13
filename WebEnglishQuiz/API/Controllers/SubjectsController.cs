using API.Common.DTOs.SubjectDTO;
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
            var subject = await _context.Subjects.Include(p => p.CategorynoNavigation).SingleOrDefaultAsync(p => p.Subjectno == subjectno && p.Status == true);

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
        public async Task<ActionResult<SubjectResponse>> PostSubject(SubjectRequest subject)
        {
            if (_context.Subjects == null)
            {
                return Problem("Entity set 'QuizAPIContext.Subjects'  is null.");
            }
            _context.Subjects.Add(_mapper.Map<Subject>(subject));
            await _context.SaveChangesAsync();

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<SubjectResponse>(subject)));
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
