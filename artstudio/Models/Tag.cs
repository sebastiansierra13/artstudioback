using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Tag
    {
        public Tag()
        {
            IdProductos = new HashSet<Producto>();
        }

        public int IdTag { get; set; }
        public string NombreTag { get; set; } = null!;
        public string? DescripcionTag { get; set; }

        public virtual ICollection<Producto> IdProductos { get; set; }
    }
}
