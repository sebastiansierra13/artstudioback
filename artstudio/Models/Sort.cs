using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Sort
    {
        public int IdSort { get; set; }
        public string NombreSort { get; set; } = null!;
        public string? DescripcionSort { get; set; }
    }
}
