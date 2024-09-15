using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Order
    {
        public Order()
        {
            Paymenttransactions = new HashSet<Paymenttransaction>();
        }

        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public string Email { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Paymenttransaction> Paymenttransactions { get; set; }
    }
}
