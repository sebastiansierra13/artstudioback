using System.ComponentModel.DataAnnotations;

namespace artstudio.DTOs
{
    public class PayUFormDTO
    {
        [Required]
        public string MerchantId { get; set; } = null!;

        [Required]
        public string ReferenceCode { get; set; } = null!;

        [Required]
        public string AccountId { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Currency { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal Amount { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El valor del impuesto no puede ser negativo.")]
        public decimal Tax { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "El valor base del impuesto no puede ser negativo.")]
        public decimal TaxReturnBase { get; set; }

        [Required]
        public string Signature { get; set; } = null!;

        [Required, EmailAddress(ErrorMessage = "El correo electrónico del comprador no es válido.")]
        public string BuyerEmail { get; set; } = null!;

        [Required, Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public string Telephone { get; set; } = null!;

        [Required]
        public string BuyerFullName { get; set; } = null!;

        [Required]
        public string ResponseUrl { get; set; } = null!;

        [Required]
        public string ConfirmationUrl { get; set; } = null!;

        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string ShippingCity { get; set; } = null!;

        [Required]
        public string ShippingCountry { get; set; } = "CO"; // Fijo en "CO"

        [Required]
        public string Test { get; set; } = null!; // "1" para pruebas, "0" para producción
    }
}
