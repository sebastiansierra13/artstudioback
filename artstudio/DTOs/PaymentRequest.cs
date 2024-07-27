namespace artstudio.DTOs
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
    }
}