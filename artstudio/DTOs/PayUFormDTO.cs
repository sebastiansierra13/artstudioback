using System.ComponentModel.DataAnnotations;

namespace artstudio.DTOs
{
    public class PayUFormDTO
    {
        public List<ProductoDTO> Productos { get; set; } = new List<ProductoDTO>(); // Lista de productos
        public string BuyerEmail { get; set; } = null!; // Correo del comprador
        public decimal Total { get; set; } // Monto total

        // Informaci�n adicional del comprador y la direcci�n
        public string FirstName { get; set; } = null!; // Nombre del comprador
        public string LastName { get; set; } = null!; // Apellido del comprador
        public string StreetName { get; set; } = null!; // Direcci�n
        public string Apartment { get; set; } = null!; // Detalles adicionales
        public string Neighborhood { get; set; } = null!; // Barrio
        public string City { get; set; } = null!; // Ciudad
        public string Department { get; set; } = null!; // Departamento
        public string Postcode { get; set; } = null!; // C�digo postal
        public string Phone { get; set; } = null!; // Tel�fono
        public string OrderNotes { get; set; } = null!; // Notas adicionales

        // Nuevas propiedades a�adidas
        public string? BuyerFullName { get; set; } = null!; // Nombre completo del comprador
        public string? ShippingAddress { get; set; } = null!; // Direcci�n de env�o
        public string? ShippingCity { get; set; } = null!; // Ciudad de env�o
        public string? ShippingCountry { get; set; } = null!; // Pa�s de env�o
        public string? MobilePhone { get; set; } = null!; // Tel�fono

        // Campos gestionados por el backend
        public string? MerchantId { get; set; }  // Identificador del comercio
        public string? AccountId { get; set; }   // Cuenta de PayU
        public string? Description { get; set; } // Descripci�n de la transacci�n
        public string? ReferenceCode { get; set; } // C�digo de referencia
        public string? Currency { get; set; } = "COP"; // Moneda
        public string? Signature { get; set; } // Firma generada
        public string? ResponseUrl { get; set; } // URL de respuesta
        public string? ConfirmationUrl { get; set; } // URL de confirmaci�n
        public string? Test { get; set; }  // Indicador de pruebas o producci�n
        public string? MerchantLogo { get; set; } // Logo del comercio
        public string? Extra1 { get; set; }   // 
        public string? Extra2 { get; set; }   // 
    }
}
