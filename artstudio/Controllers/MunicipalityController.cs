using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace artstudio.Controllers
{
    [Route("api/municipalities")]
    [ApiController]
    public class MunicipalityController : ControllerBase
    {
        private readonly MiDbContext _context;

        public MunicipalityController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/municipalities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Municipio>>> GetMunicipalities()
        {
            return await _context.Municipios.ToListAsync();
        }

        // GET: api/municipalities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Municipio>> GetMunicipality(int id)
        {
            var municipality = await _context.Municipios.FindAsync(id);

            if (municipality == null)
            {
                return NotFound();
            }

            return municipality;
        }

        // POST: api/municipalities
        [HttpPost]
        public async Task<ActionResult<Municipio>> PostMunicipality(Municipio municipality)
        {
            _context.Municipios.Add(municipality);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMunicipality), new { id = municipality.Id }, municipality);
        }

        // PUT: api/municipalities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMunicipality(int id, Municipio municipality)
        {
            if (id != municipality.Id)
            {
                return BadRequest();
            }

            _context.Entry(municipality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MunicipalityExists(id))
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

        // DELETE: api/municipalities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMunicipality(int id)
        {
            var municipality = await _context.Municipios.FindAsync(id);
            if (municipality == null)
            {
                return NotFound();
            }

            _context.Municipios.Remove(municipality);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MunicipalityExists(int id)
        {
            return _context.Municipios.Any(e => e.Id == id);
        }
    }
}
