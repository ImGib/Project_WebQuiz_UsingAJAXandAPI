using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriesController : ControllerBase
    {
        private readonly QuizAPIContext _context;

        public HistoriesController(QuizAPIContext context)
        {
            _context = context;
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
        public async Task<ActionResult<History>> PostHistory(History history)
        {
          if (_context.Histories == null)
          {
              return Problem("Entity set 'QuizAPIContext.Histories'  is null.");
          }
            _context.Histories.Add(history);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (HistoryExists(history.Htrno))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetHistory", new { id = history.Htrno }, history);
        }

        // DELETE: api/Histories/5
        [HttpDelete("{id}")]
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
