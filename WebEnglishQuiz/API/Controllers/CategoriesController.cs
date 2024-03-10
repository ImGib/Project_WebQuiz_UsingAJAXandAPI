using API.Common.DTOs.CategoryDTO;
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
    public class CategoriesController : ControllerBase
    {
        private readonly QuizAPIContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(QuizAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            List<Category> list = await _context.Categories.Include(p => p.Subjects).ToListAsync();
            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<List<CategoryResponse>>(list)));
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            var category = await _context.Categories.Include(p=>p.Subjects).SingleOrDefaultAsync(p=> p.Categoryno == id);

            if (category == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<CategoryResponse>(category)));
        }

        // PUT: api/Categories/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryRequest category)
        {
            Category? data = await _context.Categories.Include(p => p.Subjects).SingleOrDefaultAsync(p => p.Categoryno == id);

            if (data == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            data.Title = category.Title;
            data.Description = category.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<CategoryResponse>(data)));
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryRequest category)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'QuizAPIContext.Categories'  is null.");
            }
            Category data = _mapper.Map<Category>(category);
            _context.Categories.Add(data);
            await _context.SaveChangesAsync();

            return Ok(new ResponseStatus(message: ResponseOk, data: _mapper.Map<CategoryResponse>(data)));
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (_context.Categories == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return Ok(new ResponseStatus(message: ResponseError));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Categoryno == id)).GetValueOrDefault();
        }
    }
}
