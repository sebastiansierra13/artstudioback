using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace artstudio.Controllers
{
    [Route("api/banner")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly MiDbContext _context;

        public BannerController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/banner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetBanners()
        {
            return await _context.Banners.OrderBy(b => b.Posicion).ToListAsync();
        }

        // GET: api/banner/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Banner>> GetBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner == null)
            {
                return NotFound();
            }

            return banner;
        }

        // POST: api/banner
        [HttpPost]
        public async Task<ActionResult<Banner>> PostBanner(Banner banner)
        {
            // Asigna una posición si no se proporciona
            if (banner.Posicion == null)
            {
                var lastBanner = await _context.Banners.OrderByDescending(b => b.Posicion).FirstOrDefaultAsync();
                banner.Posicion = (lastBanner?.Posicion ?? 0) + 1;
            }

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBanner), new { id = banner.Id }, banner);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<Banner>>> PostBulkBanners([FromBody] List<Banner> banners)
        {
            if (banners == null || !banners.Any())
            {
                return BadRequest("No banners provided.");
            }

            var lastPosition = await _context.Banners.MaxAsync(b => b.Posicion) ?? 0;

            foreach (var banner in banners)
            {
                if (banner.Posicion == null)
                {
                    banner.Posicion = ++lastPosition;
                }
                _context.Banners.Add(banner);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBanners), banners);
        }




        [HttpPut("positions")]
        public async Task<IActionResult> UpdatePositions([FromBody] List<Banner> updates)
        {
            if (updates == null || !updates.Any())
            {
                return BadRequest("No se proporcionaron actualizaciones de posición.");
            }

            try
            {
                foreach (var update in updates)
                {
                    var banner = await _context.Banners.FindAsync(update.Id);
                    if (banner != null)
                    {
                        banner.Posicion = update.Posicion;
                        _context.Entry(banner).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();

                // Devuelve un mensaje claro de éxito
                return Ok(new { message = "Posiciones actualizadas exitosamente." });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "Ocurrió un error al actualizar las posiciones de los banners." });
            }
        }


        // PUT: api/banner/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBanner(int id, Banner banner)
        {
            if (id != banner.Id)
            {
                return BadRequest();
            }

            _context.Entry(banner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(id))
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

        // DELETE: api/banner/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
