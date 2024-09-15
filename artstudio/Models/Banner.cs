using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Banner
    {
        public int Id { get; set; }
        public int? Posicion { get; set; }
        public string? Url { get; set; }
        public string? Titulo { get; set; }
        public string? Subtitulo { get; set; }
    }
}
