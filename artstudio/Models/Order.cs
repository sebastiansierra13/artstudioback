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
        public string BuyerFullName { get; set; } = null!;
        public string MobilePhone { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string? Apartment { get; set; }
        public string Neighborhood { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string? Postcode { get; set; }
        public string? Extra1 { get; set; }
        public string? Extra2 { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = null!;
        public string OrderStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ReferenceCode { get; set; } = null!;

        public virtual ICollection<Orderproduct> Orderproducts { get; set; }
    }
}
