using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Precio
    {
        public int IdPrecio { get; set; }
        public string TamañoPoster { get; set; } = null!;
        public decimal PrecioMarco { get; set; }
        public decimal PrecioPoster { get; set; }
    }
}
