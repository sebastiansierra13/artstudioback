using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Order
    {
        public Order()
        {
            Orderproducts = new HashSet<Orderproduct>();
        }

        public int OrderId { get; set; }
        public string BuyerEmail { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ReferenceCode { get; set; } = null!;

        public virtual ICollection<Orderproduct> Orderproducts { get; set; }
    }
}
