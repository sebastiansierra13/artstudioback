namespace artstudio.DTOs
{
    public class PaymentResult
    {
        public bool IsSuccessful { get; set; } 
        public string Message { get; set; } = null!;
        public string OrderId { get; set; } = null!;
    }
}