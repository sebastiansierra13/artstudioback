namespace artstudio.DTOs
{
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
        public string permalink { get; set; } = null!;
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
