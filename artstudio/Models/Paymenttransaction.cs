using System;
using System.Collections.Generic;

namespace artstudio.Models
{
    public partial class Paymenttransaction
    {
        public int TransactionId { get; set; }
        public int? OrderId { get; set; }
        public string ReferenceCode { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string TransactionState { get; set; } = null!;
        public string? ResponseMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Order? Order { get; set; }
    }
}
