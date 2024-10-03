using System.ComponentModel.DataAnnotations;

namespace artstudio.DTOs
{
    public class PayUFormDTO
    {
        public List<ProductoDTO> Productos { get; set; } = new List<ProductoDTO>(); // Lista de productos
        public string BuyerEmail { get; set; } = null!; // Correo del comprador
        public decimal Total { get; set; } // Monto total

        // Información adicional del comprador y la dirección
        public string FirstName { get; set; } = null!; // Nombre del comprador
        public string LastName { get; set; } = null!; // Apellido del comprador
        public string StreetName { get; set; } = null!; // Dirección
        public string Apartment { get; set; } = null!; // Detalles adicionales
        public string Neighborhood { get; set; } = null!; // Barrio
        public string City { get; set; } = null!; // Ciudad
        public string Department { get; set; } = null!; // Departamento
        public string Postcode { get; set; } = null!; // Código postal
        public string Phone { get; set; } = null!; // Teléfono
        public string OrderNotes { get; set; } = null!; // Notas adicionales

        // Nuevas propiedades añadidas
        public string? BuyerFullName { get; set; } = null!; // Nombre completo del comprador
        public string? ShippingAddress { get; set; } = null!; // Dirección de envío
        public string? ShippingCity { get; set; } = null!; // Ciudad de envío
        public string? ShippingCountry { get; set; } = null!; // País de envío
        public string? MobilePhone { get; set; } = null!; // Teléfono

        // Campos gestionados por el backend
        public string? MerchantId { get; set; }  // Identificador del comercio
        public string? AccountId { get; set; }   // Cuenta de PayU
        public string? Description { get; set; } // Descripción de la transacción
        public string? ReferenceCode { get; set; } // Código de referencia
        public string? Currency { get; set; } = "COP"; // Moneda
        public string? Signature { get; set; } // Firma generada
        public string? ResponseUrl { get; set; } // URL de respuesta
        public string? ConfirmationUrl { get; set; } // URL de confirmación
        public string? Test { get; set; }  // Indicador de pruebas o producción
        public string? MerchantLogo { get; set; } // Logo del comercio
        public string? Extra1 { get; set; }   // 
        public string? Extra2 { get; set; }   // 
    }
}
