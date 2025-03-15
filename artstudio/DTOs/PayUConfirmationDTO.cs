namespace artstudio.DTOs
{
    public class PayUConfirmationDTO
    {
        public string Merchant_id { get; set; } = null!;
        public string Reference_sale { get; set; } = null!;
        public string Value { get; set; } = null!;  // Change to string to preserve formatting
        public string Currency { get; set; } = null!;
        public string Sign { get; set; } = null!;
        public string State_pol { get; set; } = null!;
        public string Payment_method_name { get; set; } = null!;
        public string Transaction_id { get; set; } = null!;
        public string Email_buyer { get; set; } = null!;
        public string? Additional_value { get; set; }
        public string? Description { get; set; }
        public string? Payment_method_id { get; set; }
        public string? Pse_bank { get; set; }
        public string? Risk { get; set; }
        public string? Attempts { get; set; }
        public string? Shipping_city { get; set; }
        public string? Shipping_country { get; set; }
    }

}
