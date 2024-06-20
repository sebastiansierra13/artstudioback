using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace artstudio.Models
{
    public partial class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public int IdCategoria { get; set; }
        public string Imagenes { get; set; } = null!;
        public string? DescripcionProducto { get; set; }
        public string? ListPrecios { get; set; }
        public string? ListTags { get; set; }
        public int CantVendido { get; set; }

        [JsonIgnore]
        public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
    }
}
