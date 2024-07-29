using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Instagramtoken
    {
        public int Id { get; set; }
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
