namespace artstudio.DTOs
{
    public class PaymentConfirmation
    {
        public string TransactionId { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public string SignatureReceived { get; set; } = null!;
    }
}