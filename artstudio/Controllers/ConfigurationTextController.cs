using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace artstudio.Controllers
{
    [Route("api/configurationtext")]
    [ApiController]
    public class ConfigurationTextController : ControllerBase
    {
        private readonly MiDbContext _context;

        public ConfigurationTextController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/configurationtext/{section}
        [HttpGet("{section}")]
        public async Task<ActionResult<Configurationtext>> GetText(string section)
        {
            var text = await _context.Configurationtexts.SingleOrDefaultAsync(ct => ct.Section == section);

            if (text == null)
            {
                return NotFound(new { message = $"Text for section '{section}' not found." });
            }

            return Ok(text);
        }

        // POST: api/configurationtext
        [HttpPost]
        public async Task<ActionResult<Configurationtext>> PostText(Configurationtext configurationText)
        {
            var existingText = await _context.Configurationtexts
                .SingleOrDefaultAsync(ct => ct.Section == configurationText.Section);

            if (existingText != null)
            {
                return Conflict(new { message = $"Text for section '{configurationText.Section}' already exists." });
            }

            _context.Configurationtexts.Add(configurationText);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetText), new { section = configurationText.Section }, configurationText);
        }

        // PUT: api/configurationtext/{section}
        [HttpPut("{section}")]
        public async Task<IActionResult> PutText(string section, Configurationtext configurationText)
        {
            if (section != configurationText.Section)
            {
                return BadRequest(new { message = "Section mismatch in PUT request." });
            }

            var existingText = await _context.Configurationtexts.SingleOrDefaultAsync(ct => ct.Section == section);
            if (existingText == null)
            {
                return NotFound(new { message = $"Text for section '{section}' not found." });
            }

            existingText.TextContent = configurationText.TextContent;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
