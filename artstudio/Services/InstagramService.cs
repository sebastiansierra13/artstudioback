using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using artstudio.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using artstudio.DTOs;

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

        public InstagramService(HttpClient httpClient, MiDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<List<InstagramPostDTO>> GetLatestPostsAsync()
        {
            var token = await _context.Instagramtokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (token == null || token.ExpiryDate <= DateTime.UtcNow)
            {
                throw new Exception("Token de acceso inválido o expirado.");
            }

            try
            {
                var response = await _httpClient.GetAsync($"https://graph.instagram.com/me/media?fields=id,caption,media_type,media_url,timestamp,children{{media_type,media_url}}&access_token={token.AccessToken}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Contenido de la respuesta: " + content);

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
                        CreatedAt = DateTime.Parse(p.timestamp)
                    })
                    .Where(p => p.ImageUrl != null)
                    .Take(5)
                    .ToList();

                Console.WriteLine($"Número de posts procesados: {posts.Count}");
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
    }

    public class InstagramApiResponse
    {
        public List<InstagramPost> Data { get; set; } = new List<InstagramPost>();
    }

    public class InstagramPost
    {
        public string id { get; set; } = null!;
        public string caption { get; set; } = null!;
        public string media_type { get; set; } = null!;
        public string media_url { get; set; } = null!;
        public string timestamp { get; set; } = null!;
        public InstagramChildrenResponse? children { get; set; }
    }

    public class InstagramChildrenResponse
    {
        public List<InstagramChild> Data { get; set; } = new List<InstagramChild>();
    }

    public class InstagramChild
    {
        public string media_type { get; set; } = null!;
        public string media_url { get; set; } = null!;
    }

}