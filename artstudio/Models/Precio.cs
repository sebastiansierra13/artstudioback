using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Precio
    {
        public int IdPrecio { get; set; }
        public string? TamanhoPoster { get; set; }
        public decimal PrecioMarco { get; set; }
        public decimal PrecioPoster { get; set; }
    }
}
