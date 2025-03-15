using System.ComponentModel.DataAnnotations;

namespace artstudio.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string OrderId { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0.")]
        public decimal Total { get; set; }

        [Required, EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = null!;

        [Required, MinLength(10, ErrorMessage = "La dirección debe tener al menos 10 caracteres.")]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string ShippingCity { get; set; } = null!;

        [Required]
        public string BuyerFullName { get; set; } = null!;

        [Required, Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public string Telephone { get; set; } = null!;

        [Required, EmailAddress(ErrorMessage = "El correo electrónico del pagador no es válido.")]
        public string PayerEmail { get; set; } = null!;

        [Required, Phone(ErrorMessage = "El número de teléfono del pagador no es válido.")]
        public string PayerPhone { get; set; } = null!;

        [Required]
        public string PayerFullName { get; set; } = null!;

        [Required]
        public string PayerDocument { get; set; } = null!;

        [Required]
        public string PayerDocumentType { get; set; } = null!;
    }
}
