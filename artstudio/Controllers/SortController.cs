using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace artstudio.Controllers
{
    [Route("api/sorts")]
    [ApiController]
    public class SortController : ControllerBase
    {
        private readonly MiDbContext _context;

        public SortController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/sorts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sort>>> GetSorts()
        {
            return await _context.Sorts.ToListAsync();
        }

        // GET: api/sorts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sort>> GetSort(int id)
        {
            var sort = await _context.Sorts.FindAsync(id);

            if (sort == null)
            {
                return NotFound();
            }

            return sort;
        }

        // POST: api/sorts
        [HttpPost]
        public async Task<ActionResult<Sort>> PostSort(Sort sort)
        {
            _context.Sorts.Add(sort);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSort), new { id = sort.IdSort }, sort);
        }

        // PUT: api/sorts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSort(int id, Sort sort)
        {
            if (id != sort.IdSort)
            {
                return BadRequest();
            }

            _context.Entry(sort).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SortExists(id))
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

        // DELETE: api/sorts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSort(int id)
        {
            var sort = await _context.Sorts.FindAsync(id);
            if (sort == null)
            {
                return NotFound();
            }

            _context.Sorts.Remove(sort);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SortExists(int id)
        {
            return _context.Sorts.Any(e => e.IdSort == id);
        }
    }
}
