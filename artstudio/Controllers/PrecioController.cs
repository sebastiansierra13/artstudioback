using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace artstudio.Controllers
{
    [Route("api/precios")]
    [ApiController]
    public class PrecioController : ControllerBase
    {
        private readonly MiDbContext _context;

        public PrecioController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/precios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Precio>>> GetPrecios()
        {
            return await _context.Precios.ToListAsync();
        }

        // GET: api/precios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Precio>> GetPrecio(int id)
        {
            var precio = await _context.Precios.FindAsync(id);

            if (precio == null)
            {
                return NotFound();
            }

            return precio;
        }

        // POST: api/precios
        [HttpPost]
        public async Task<ActionResult<Precio>> PostPrecio(Precio precio)
        {
            _context.Precios.Add(precio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrecio), new { id = precio.IdPrecio }, precio);
        }

        // PUT: api/precios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecio(int id, Precio precio)
        {
            if (id != precio.IdPrecio)
            {
                return BadRequest();
            }

            _context.Entry(precio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioExists(id))
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

        // DELETE: api/precios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecio(int id)
        {
            var precio = await _context.Precios.FindAsync(id);
            if (precio == null)
            {
                return NotFound();
            }

            _context.Precios.Remove(precio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrecioExists(int id)
        {
            return _context.Precios.Any(e => e.IdPrecio == id);
        }
    }
}
