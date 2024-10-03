using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Orderproduct
    {
        public int OrderProductId { get; set; }
        public int OrderId { get; set; }
        public long? ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string TamanhoPoster { get; set; } = null!;
        public decimal PrecioPoster { get; set; }
        public decimal PrecioMarco { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public string? ProductImageUrl { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
