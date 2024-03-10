using API.Common.DTOs.AnswerDTO;
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
    public class AnswersController : ControllerBase
    {
        private readonly QuizAPIContext _context;
        private readonly IMapper _mapper;

        public AnswersController(QuizAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Answers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerResponse>>> GetAnswers()
        {
            if (_context.Answers == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            List<Answer> list = await _context.Answers.ToListAsync();
            return _mapper.Map<List<AnswerResponse>>(list);
        }

        // GET: api/Answers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnswerResponse>> GetAnswer(int id)
        {
            if (_context.Answers == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            var answer = await _context.Answers.FindAsync(id);

            if (answer == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<AnswerResponse>(answer)));
        }

        // PUT: api/Answers/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswer(int id, AnswerRequestBase answer)
        {
            Answer? data = await _context.Answers.FindAsync(id);
            if (data == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            data.Questionno = answer.Questionno;
            data.Description = answer.Description;
            data.Iscorect = answer.Iscorect;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(id))
                {
                    return Ok(new ResponseStatus(message: ResponseError));
                }
                else
                {
                    throw;
                }
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<AnswerResponse>(data)));
        }

        // POST: api/Answers

        [HttpPost]
        public async Task<ActionResult<Answer>> PostAnswer(AnswerRequestBase answer)
        {
            if (_context.Answers == null)
            {
                return Problem("Entity set 'QuizAPIContext.Answers'  is null.");
            }
            _context.Answers.Add(_mapper.Map<Answer>(answer));
            await _context.SaveChangesAsync();

            return Ok(new ResponseStatus(message: ResponseOk));
        }

        // DELETE: api/Answers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            if (_context.Answers == null)
            {
                return NotFound();
            }
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnswerExists(int id)
        {
            return (_context.Answers?.Any(e => e.Answerno == id)).GetValueOrDefault();
        }
    }
}
