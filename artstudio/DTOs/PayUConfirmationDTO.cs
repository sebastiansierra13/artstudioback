namespace artstudio.DTOs
{
    public class PayUConfirmationDTO
    {
        public string MerchantId { get; set; } = null!;
        public string ReferenceSale { get; set; } = null!;
        public string TransactionState { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string BuyerEmail { get; set; } = null!;
        public string PayerEmail { get; set; } = null!;
    }
}
