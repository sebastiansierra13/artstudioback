using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Tag
    {
        public int IdTag { get; set; }
        public string NombreTag { get; set; } = null!;
        public string? DescripcionTag { get; set; }
    }
}
