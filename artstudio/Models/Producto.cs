
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace artstudio.Models
{
    public partial class Producto
    {
        public Producto()
        {
            IdTags = new HashSet<Tag>();
        }

        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public int IdCategoria { get; set; }
        public string Imagenes { get; set; } = null!;
        public string? DescripcionProducto { get; set; }
        public string ListTags { get; set; } = null!;
        public int CantVendido { get; set; }
        public int? Posicion { get; set; }
        public bool? Destacado { get; set; }

        [JsonIgnore]
        public virtual Categoria? IdCategoriaNavigation { get; set; }

        public virtual ICollection<Tag> IdTags { get; set; }
    }
}


