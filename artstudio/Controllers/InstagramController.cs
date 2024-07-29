using artstudio.DTOs;
using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using artstudio.Services;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http; 
using Newtonsoft.Json;


namespace artstudio.Controllers
{


    [ApiController]
    [Route("instagram")]
    public class InstagramController : ControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly MiDbContext _context;

        public InstagramController(IInstagramService instagramService, IConfiguration configuration, HttpClient httpClient, MiDbContext context)
        {
            _instagramService = instagramService;
            _context = context;
        }

        [HttpGet("latest-posts")]
        public async Task<IActionResult> GetLatestPostsAsync()
        {
            try
            {
                var posts = await _instagramService.GetLatestPostsAsync();
                Console.WriteLine($"Número de posts recuperados: {posts.Count}");
                Console.WriteLine($"Primer post: {JsonConvert.SerializeObject(posts.FirstOrDefault())}");
                return Ok(posts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetLatestPostsAsync: {ex}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        // Endpoint para la solicitud de eliminación de datos
        [HttpPost("delete-data")]
        public IActionResult DeleteData([FromBody] DeleteDataRequest request)
        {
            // Simplemente devuelve un mensaje indicando que no se almacenan datos personales
            return Ok(new { status = "No se almacenan datos personales de los usuarios." });
        }



        [HttpGet("auth")]
        public IActionResult Authenticate()
        {
            var clientId = _configuration["Instagram:ClientId"];
            var redirectUri = _configuration["Instagram:RedirectUri"];
            var authUrl = $"https://api.instagram.com/oauth/authorize?client_id={clientId}&redirect_uri={redirectUri}&scope=user_profile,user_media&response_type=code";
            return Redirect(authUrl);
        }


        [HttpPost("store-token")]
        public async Task<IActionResult> StoreToken([FromBody] string accessToken)
        {
            var token = new Instagramtoken
            {
                AccessToken = accessToken,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(60) // Suponiendo que el token expira en 60 días
            };

            _context.Instagramtokens.Add(token);
            await _context.SaveChangesAsync();

            return Ok("Token almacenado exitosamente.");
        }



        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var clientId = _configuration["Instagram:ClientId"];
            var clientSecret = _configuration["Instagram:ClientSecret"];
            var redirectUri = _configuration["Instagram:RedirectUri"];

            var requestUrl = "https://api.instagram.com/oauth/access_token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("code", code)
            });

            var response = await _httpClient.PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseString);
            var accessToken = responseJson["access_token"]?.ToString();

            // Save accessToken securely, e.g., in a database or a secure storage
            // ...

            return Ok("Authentication successful, access token received.");
        }
    }
}
