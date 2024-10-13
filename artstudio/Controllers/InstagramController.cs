using artstudio.DTOs;
using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using artstudio.Services;
using Microsoft.Extensions.Options;
using artstudio.Configuration;

namespace artstudio.Controllers
{
    [ApiController]
    [Route("instagram")]
    public class InstagramController : ControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly InstagramSettings _instagramSettings;
        private readonly MiDbContext _context;

        public InstagramController(
            IInstagramService instagramService,
            IOptions<InstagramSettings> instagramSettings,
            MiDbContext context)
        {
            _instagramService = instagramService;
            _instagramSettings = instagramSettings.Value;
            _context = context;
        }

        [HttpGet("latest-posts")]
        public async Task<IActionResult> GetLatestPostsAsync()
        {
            try
            {
                var posts = await _instagramService.GetLatestPostsAsync();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetLatestPostsAsync: {ex}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("delete-data")]
        public IActionResult DeleteData([FromBody] DeleteDataRequest request)
        {
            return Ok(new { status = "No se almacenan datos personales de los usuarios." });
        }

        public class TokenRequest
        {
            public string AccessToken { get; set; }
        }

        [HttpPost("store-token")]
        public async Task<IActionResult> StoreToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request?.AccessToken))
            {
                return BadRequest("AccessToken is required.");
            }

            var token = new Instagramtoken
            {
                AccessToken = request.AccessToken,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };
            _context.Instagramtokens.Add(token);
            await _context.SaveChangesAsync();
            return Ok("Token almacenado exitosamente.");
        }
    }
    }
