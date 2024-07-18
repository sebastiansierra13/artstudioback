using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Municipio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int CodigoDane { get; set; }
        public int? DepartamentoId { get; set; }

        public virtual Departamento? Departamento { get; set; }
    }
}
