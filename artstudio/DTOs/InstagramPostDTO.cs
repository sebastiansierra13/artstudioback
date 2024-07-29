namespace artstudio.DTOs
{
    public class InstagramPostDTO
    {
        public string Id { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Caption { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}