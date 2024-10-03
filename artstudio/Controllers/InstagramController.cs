using artstudio.DTOs;
using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using artstudio.Services;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using artstudio.Configuration;

namespace artstudio.Controllers
{
    [ApiController]
    [Route("instagram")]
    public class InstagramController : ControllerBase
    {
        private readonly IInstagramService _instagramService;
        private readonly InstagramSettings _instagramSettings;
        private readonly HttpClient _httpClient;
        private readonly MiDbContext _context;

        public InstagramController(
            IInstagramService instagramService,
            IOptions<InstagramSettings> instagramSettings,
            HttpClient httpClient,
            MiDbContext context)
        {
            _instagramService = instagramService;
            _instagramSettings = instagramSettings.Value;
            _httpClient = httpClient;
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

        [HttpGet("auth")]
        public IActionResult Authenticate()
        {
            var authUrl = $"https://api.instagram.com/oauth/authorize?client_id={_instagramSettings.ClientId}&scope=user_profile,user_media&response_type=code";
            return Redirect(authUrl);
        }

        [HttpPost("store-token")]
        public async Task<IActionResult> StoreToken([FromBody] string accessToken)
        {
            var token = new Instagramtoken
            {
                AccessToken = accessToken,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };

            _context.Instagramtokens.Add(token);
            await _context.SaveChangesAsync();

            return Ok("Token almacenado exitosamente.");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var requestUrl = "https://api.instagram.com/oauth/access_token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _instagramSettings.ClientId),
                new KeyValuePair<string, string>("client_secret", _instagramSettings.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code)
            });

            var response = await _httpClient.PostAsync(requestUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseString);
            var accessToken = responseJson["access_token"]?.ToString();

            // Aquí podrías llamar a StoreToken para guardar el token
            // await StoreToken(accessToken);

            return Ok("Authentication successful, access token received.");
        }
    }
}