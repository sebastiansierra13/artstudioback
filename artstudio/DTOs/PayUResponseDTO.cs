namespace artstudio.DTOs
{
    public class PayUResponseDTO
    {
        public string MerchantId { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string TX_VALUE { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string TransactionState { get; set; } = null!;
        public string Signature { get; set; } = null!;
        public string LapPaymentMethod { get; set; } = null!;
        public string LapTransactionState { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? ReferencePol { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? TrazabilityCode { get; set; } = null!;
        public string? Cus { get; set; } = null!;
        public string OrderLanguage { get; set; } = null!;
        public string? Extra1 { get; set; } = null!;
        public string? Extra2 { get; set; } = null!;
        public string? Extra3 { get; set; } = null!;
        public string PolTransactionState { get; set; } = null!;
        public string PolResponseCode { get; set; } = null!;
        public string LapResponseCode { get; set; } = null!;
        public string? Risk { get; set; } = null!;
        public string PolPaymentMethod { get; set; } = null!;
        public string PolPaymentMethodType { get; set; } = null!;
        public string LapPaymentMethodType { get; set; } = null!;
        public string InstallmentsNumber { get; set; } = null!;
        public string Lng { get; set; } = null!;
        public string BuyerEmail { get; set; } = null!;
        public string? PseBank { get; set; } = null!;
        public string? AuthorizationCode { get; set; } = null!;
        public decimal TX_TAX { get; set; }
        public decimal TX_ADMINISTRATIVE_FEE { get; set; }
        public decimal TX_TAX_ADMINISTRATIVE_FEE { get; set; }
        public decimal TX_TAX_ADMINISTRATIVE_FEE_RETURN_BASE { get; set; }
        public DateTime ProcessingDate { get; set; }
    }

}
