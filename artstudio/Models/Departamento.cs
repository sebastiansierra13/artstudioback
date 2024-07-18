using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Departamento
    {
        public Departamento()
        {
            Municipios = new HashSet<Municipio>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CodigoDane { get; set; }
        public int? RegionId { get; set; }

        public virtual Region? Region { get; set; }
        public virtual ICollection<Municipio> Municipios { get; set; }
    }
}
