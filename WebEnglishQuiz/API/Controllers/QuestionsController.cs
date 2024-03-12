using API.Common.DTOs.QuestionDTO;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static API.Common.Utility;
using static API.Common.Variables;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class QuestionsController : ControllerBase
	{
		private readonly QuizAPIContext _context;
		private readonly IMapper _mapper;

		public QuestionsController(QuizAPIContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// GET: api/Questions
		[HttpGet]
		public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestions()
		{
			if (_context.Questions == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}
			List<Question> list = await _context.Questions.Include(p => p.Answers).ToListAsync();
			return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<QuestionResponse>>(list)));
		}

		[HttpGet("Public")]
		public async Task<ActionResult<IEnumerable<QuestionResponse>>> PublicQuestions()
		{
			if (_context.Questions == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}
			List<Question> list = await _context.Questions.Include(p => p.Answers).Where(p => p.Status == true).ToListAsync();
			return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<QuestionResponse>>(list)));
		}
        [HttpGet("CreateExam")]
        public async Task<ActionResult<IEnumerable<QuestionResponse>>> CreateExam(int subjectid)
        {
            if (_context.Questions == null || _context.Subjects == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

			//get list subject's questions
			List<Question> dataList = await _context.Questions.Include(p=>p.Answers).Where(p => p.Subjectno ==  subjectid).ToListAsync();

            //get random 10 questions
            Random random = new Random();
			Question temp;
            // Fisher-Yates shuffle algorithm
            for (int i = dataList.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                temp = dataList[i];
                dataList[i] = dataList[j];
                dataList[j] = temp;
            }

            List<Question> randomSublist = dataList.GetRange(0, numberQuestion);
            //return data

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<QuestionResponse>>(randomSublist)));
        }
        // GET: api/Questions/5
        [HttpGet("{id}")]
		public async Task<ActionResult<QuestionResponse>> GetQuestion(int id)
		{
			if (_context.Questions == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}
			var question = await _context.Questions.Include(p => p.Answers).FirstOrDefaultAsync(p => p.Questionno == id);

			if (question == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}

			return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<QuestionResponse>(question)));
		}

		// PUT: api/Questions/5

		[HttpPut("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> PutQuestion(int id, QuestionRequestBase question)
		{
			Question? data = await _context.Questions.FirstOrDefaultAsync(p => p.Questionno == id);

			if (data == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}

			data.Description = question.Description;
			data.Subjectno = question.Subjectno;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!QuestionExists(id))
				{
					return Ok(new ResponseStatus(message: ResponseError));
				}
				else
				{
					throw;
				}
			}

			return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<QuestionResponse>(data)));
		}

		// POST: api/Questions

		[HttpPost]
		//[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Question>> PostQuestion(QuestionRequestBase question)
		{
			if (_context.Questions == null)
			{
				return Problem("Entity set 'QuizAPIContext.Questions'  is null.");
			}
			Question data = _mapper.Map<Question>(question);
			_context.Questions.Add(data);
			await _context.SaveChangesAsync();

			return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<QuestionResponse>(data)));
		}

		// DELETE: api/Questions/5
		[HttpDelete("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteQuestion(int id)
		{
			if (_context.Questions == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}
			var question = await _context.Questions.FindAsync(id);
			if (question == null)
			{
				return Ok(new ResponseStatus(message: ResponseError));
			}

			_context.Questions.Remove(question);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool QuestionExists(int id)
		{
			return (_context.Questions?.Any(e => e.Questionno == id)).GetValueOrDefault();
		}
	}
}
