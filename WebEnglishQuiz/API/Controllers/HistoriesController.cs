using API.Common;
using API.Common.DTOs.AnswerDTO;
using API.Common.DTOs.HistoryDTO;
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
    public class HistoriesController : ControllerBase
    {
        private readonly QuizAPIContext _context;
        private readonly IMapper _mapper;
        public HistoriesController(QuizAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Histories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<History>>> GetHistories()
        {
            if (_context.Histories == null)
            {
                return NotFound();
            }
            return await _context.Histories.ToListAsync();
        }

        // GET: api/Histories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetHistory(int id)
        {
            if (_context.Histories == null)
            {
                return NotFound();
            }
            var history = await _context.Histories.FindAsync(id);

            if (history == null)
            {
                return NotFound();
            }

            return history;
        }

        // PUT: api/Histories/5

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutHistory(int id, History history)
        {
            if (id != history.Htrno)
            {
                return BadRequest();
            }

            _context.Entry(history).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Histories

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseStatus>> PostHistory(HistoryRequestBase data)
        {
            if (_context.Histories == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            if (data == null || data.UserName == null || data.answerList == null)
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            //get test no max
            int testno = 0;
            var alluser = _context.Histories.GroupBy(p => p.Username)
                        .Select(group => new
                        {
                            UserName = group.Key,
                            TestNo = group.Max(row => row.Testno)
                        });
            try
            {
                testno = alluser.Where(p => p.UserName.ToUpper().Trim().Equals(data.UserName.ToUpper().Trim()))
                        .Select(p => p.TestNo).FirstOrDefault();
            }
            catch
            {
                return Ok(new ResponseStatus("Get test no fail"));
            }

            testno++;

            string[] anslist = data.answerList.Split(",");

            //number correct ans
            int correct = 0;
            try
            {

                //add data
                foreach (string id in anslist)
                {
                    int ansno = int.Parse(id);

                    //find question no
                    Answer? answer = _context.Answers.Find(ansno);

                    if (answer == null)
                    {
                        return Ok(new ResponseStatus(ResponseError, _mapper.Map<AnswerResponse>(answer)));
                    }

                    int quesno = answer.Questionno;
                    //check correct
                    correct = correct + (answer.Iscorect ? 1 : 0);

                    //add data
                    _context.Histories.Add(new History
                    {
                        Username = data.UserName,
                        Testno = testno,
                        Answerno = ansno,
                        Questionno = quesno,
                        Subjectno = data.SubjectNo
                    });
                }
                //save data added
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Ok(new ResponseStatus(ResponseError));
            }

            return Ok(new ResponseStatus(ResponseOk, new Utility.CorrectAnswer
            {
                Correct = correct,
                Question = anslist.Length
            }));
        }

        // DELETE: api/Histories/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            if (_context.Histories == null)
            {
                return NotFound();
            }
            var history = await _context.Histories.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            _context.Histories.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistoryExists(int id)
        {
            return (_context.Histories?.Any(e => e.Htrno == id)).GetValueOrDefault();
        }
    }
}
