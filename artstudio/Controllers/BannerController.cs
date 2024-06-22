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

        public BannerController (MiDbContext context)
        {
            _context = context;
        }

        // GET: api/Banner
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Banner>>> GetBanner()
        {
            return await _context.Banners.ToListAsync();
        }

        // GET: api/Banner/5
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
        public async Task<ActionResult<Banner>> PostTag(Banner banner)
        {
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBanner), new { id = banner.Id },banner);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<Banner>>> PostBulkBanners(IEnumerable<string> urls)
        {
            if (urls == null || !urls.Any())
            {
                return BadRequest("No URLs provided.");
            }

            List<Banner> bannersToAdd = new List<Banner>();

            foreach (var url in urls)
            {
                // Crear un nuevo objeto Banner con la URL proporcionada
                Banner banner = new Banner
                {
                    Url = url
                };

                bannersToAdd.Add(banner);
            }

            // Agregar todos los banners a la base de datos y guardar cambios
            _context.Banners.AddRange(bannersToAdd);
            await _context.SaveChangesAsync();

            // Retornar un ActionResult con todos los banners agregados
            return CreatedAtAction(nameof(GetBulkBanners), bannersToAdd);
        }

        // Otro método para obtener los banners (ejemplo)
        [HttpGet("bulk")]
        public async Task<ActionResult<IEnumerable<Banner>>> GetBulkBanners()
        {
            return await _context.Banners.ToListAsync();
        }
    

    // PUT: api/tags/5
    [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, Tag tag)
        {
            if (id != tag.IdTag)
            {
                return BadRequest();
            }

            _context.Entry(tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExist(id))
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

        private bool BannerExist(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
