using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Region
    {
        public Region()
        {
            Departamentos = new HashSet<Departamento>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Departamento> Departamentos { get; set; }
    }
}
