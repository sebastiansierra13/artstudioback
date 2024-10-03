using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using artstudio.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using artstudio.DTOs; // Asegúrate de tener esta directiva
using artstudio.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace artstudio.Services
{
    public interface IInstagramService
    {
        Task<List<InstagramPostDTO>> GetLatestPostsAsync();
    }

    public class InstagramService : IInstagramService
    {
        private readonly HttpClient _httpClient;
        private readonly MiDbContext _context;
        private readonly InstagramSettings _instagramSettings;

        // Cambia el constructor para recibir IOptions<InstagramSettings>
        public InstagramService(HttpClient httpClient, MiDbContext context, IOptions<InstagramSettings> instagramSettings)
        {
            _httpClient = httpClient;
            _context = context;
            _instagramSettings = instagramSettings.Value;  // Obtiene el valor real de IOptions
        }

        public async Task<List<InstagramPostDTO>> GetLatestPostsAsync()
        {
            var token = await _context.Instagramtokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (token == null)
            {
                throw new Exception("No se encontró un token de acceso.");
            }

            // Verificar si el token ha expirado o está por expirar en las próximas 24 horas
            if (token.ExpiryDate <= DateTime.UtcNow || (token.ExpiryDate - DateTime.UtcNow).TotalHours < 24)
            {
                // Intentar actualizar el token
                token = await RefreshAccessTokenAsync(token);
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://graph.instagram.com/me/media?fields=id,caption,media_type,media_url,timestamp,permalink,children{{media_type,media_url}}&access_token={token.AccessToken}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<InstagramApiResponse>(content);
                if (result == null || result.Data == null || !result.Data.Any())
                {
                    throw new Exception("No se recibieron datos válidos de la API de Instagram.");
                }

                var posts = result.Data
                    .Where(p => p.media_type == "CAROUSEL_ALBUM" || p.media_type == "IMAGE")
                    .Select(p => new InstagramPostDTO
                    {
                        Id = p.id,
                        ImageUrl = p.media_type == "IMAGE" ? p.media_url : p.children?.Data?.FirstOrDefault(c => c.media_type == "IMAGE")?.media_url,
                        Caption = p.caption,
                        CreatedAt = DateTime.Parse(p.timestamp),
                        PostUrl = p.permalink
                    })
                    .Where(p => p.ImageUrl != null)
                    .Take(10)
                    .ToList();

                return posts;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al comunicarse con la API de Instagram: {ex.Message}");
                throw new Exception("Error al comunicarse con la API de Instagram.", ex);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error al procesar la respuesta de la API de Instagram: {ex.Message}");
                throw new Exception("Error al procesar la respuesta de la API de Instagram.", ex);
            }
        }

        private async Task<Instagramtoken> RefreshAccessTokenAsync(Instagramtoken currentToken)
        {
            var requestUrl = $"https://graph.instagram.com/refresh_access_token?grant_type=ig_refresh_token&access_token={currentToken.AccessToken}";

            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error al actualizar el token de acceso.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<JObject>(content);
            var newAccessToken = responseJson["access_token"]?.ToString();
            var expiresInSeconds = responseJson["expires_in"]?.ToObject<int>() ?? 5184000; // Por defecto 60 días

            if (string.IsNullOrEmpty(newAccessToken))
            {
                throw new Exception("Error al obtener un nuevo token de acceso.");
            }

            // Actualizar el token existente con los nuevos valores
            currentToken.AccessToken = newAccessToken;
            currentToken.ExpiryDate = DateTime.UtcNow.AddSeconds(expiresInSeconds);
            currentToken.CreatedAt = DateTime.UtcNow;

            // Guardar el token actualizado en la base de datos
            _context.Instagramtokens.Update(currentToken);
            await _context.SaveChangesAsync();

            return currentToken;
        }
    }
}
