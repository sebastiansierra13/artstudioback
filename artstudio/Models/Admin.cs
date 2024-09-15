using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Admin
    {
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Id { get; set; }
    }
}
