using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Configurationtext
    {
        public int Id { get; set; }
        public string Section { get; set; } = null!;
        public string TextContent { get; set; } = null!;
    }
}
