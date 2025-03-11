using artstudio.Configuration;
using artstudio.DTOs;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System;

namespace artstudio.Services
{
    public interface IPayUService
    {
        // Método para generar la firma para el formulario de pago
        string GenerateSignature(PayUFormDTO formData);

        // Método para generar el formulario de pago
        PayUFormDTO GeneratePaymentForm(PayUFormDTO formData);

        // Método para validar la firma de la confirmación de PayU (IPN)
        bool ValidateSignature(PayUConfirmationDTO confirmationDTO);

        // Método para validar la firma de la respuesta de PayU (Página de respuesta)
        bool ValidateSignature(PayUResponseDTO responseDTO);

    }


    public class PayUService : IPayUService
    {
        private readonly PayUSettings _payUSettings;

        public PayUService(IOptions<PayUSettings> payUSettings)
        {
            _payUSettings = payUSettings.Value;


            // Imprimir las variables de entorno cargadas en PayUSettings
            Console.WriteLine("---- PayUSettings Cargadas ----");
            Console.WriteLine($"MerchantId: {_payUSettings.MerchantId}");
            Console.WriteLine($"AccountId: {_payUSettings.AccountId}");
            Console.WriteLine($"ApiKey: {_payUSettings.ApiKey}");
            Console.WriteLine($"ResponseUrl: {_payUSettings.ResponseUrl}");
            Console.WriteLine($"ConfirmationUrl: {_payUSettings.ConfirmationUrl}");
            Console.WriteLine($"TestMode: {_payUSettings.TestMode}");
            Console.WriteLine($"SandboxUrl: {_payUSettings.SandboxUrl}");
            Console.WriteLine($"ProductionUrl: {_payUSettings.ProductionUrl}");
            Console.WriteLine("--------------------------------");
        }

        // Método para generar el código de referencia único
        public string GenerateReferenceCode()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss"); // Fecha y hora actuales
            var guid = Guid.NewGuid().ToString().Substring(0, 8); // Genera un GUID parcial

            return $"Order-{timestamp}-{guid}"; // Ejemplo de código: "Order-20230922124530-f7a1d2b3"
        }

        // Generar la firma de PayU
        public string GenerateSignature(PayUFormDTO formData)
        {
            // Genera el string que será hasheado
            var signatureString = $"{_payUSettings.ApiKey}~{formData.MerchantId}~{formData.ReferenceCode}~{formData.Total.ToString("F2", CultureInfo.InvariantCulture)}~{formData.Currency}";

            // Log para depuración
            Console.WriteLine("String para la firma: " + signatureString);

            // Aplica MD5 para generar la firma
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(signatureString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convertir a formato hexadecimal en minúsculas
            }
        }



        // Generar el formulario de pago con los datos proporcionados
        public PayUFormDTO GeneratePaymentForm(PayUFormDTO formData)
        {
            // Completar datos sensibles que vienen del backend
            formData.MerchantId = _payUSettings.MerchantId;
            formData.AccountId = _payUSettings.AccountId;
            formData.Currency = "COP";
            formData.ResponseUrl = _payUSettings.ResponseUrl;
            formData.ConfirmationUrl = _payUSettings.ConfirmationUrl;
            formData.Description = "Compra de productos";
            formData.ReferenceCode = GenerateReferenceCode();  // Genera un código de referencia único
            // Asignar datos requeridos para PayU
            formData.ShippingAddress = formData.StreetName; // Dirección de envío
            formData.ShippingCity = formData.City; // Ciudad de envío
            formData.Postcode = formData.Postcode; // Código postal
            formData.ShippingCountry = "CO"; // Código ISO de Colombia
            formData.BuyerEmail = formData.BuyerEmail; // Correo del comprador
            formData.MobilePhone = formData.Phone; // Teléfono del comprador (opcional)
            formData.FirstName = formData.FirstName; // Primer nombre
            formData.LastName = formData.LastName; // Apellido

            // Combina nombre y apellido para buyerFullName
            formData.BuyerFullName = $"{formData.FirstName} {formData.LastName}";

            // Verifica que el Total tenga un valor y no esté vacío
            if (formData.Total <= 0)
            {
                throw new Exception("El monto total debe ser mayor a 0.");
            }

            // Usar el campo Extra1 para pasar el logo del comercio
            formData.MerchantLogo = "https://firebasestorage.googleapis.com/v0/b/fireartstudio-8586e.appspot.com/o/images%2Flogo%2FlogoArtStudioO.png?alt=media&token=f4d3707b-88fe-4e82-8a11-7aef0a7bcc0e";
            formData.Extra1 = formData.Apartment; //
            formData.Extra2 = formData.OrderNotes; // 

            // Determinar si estamos en modo producción o sandbox según las variables de entorno
            formData.PayUUrl = _payUSettings.TestMode ? _payUSettings.SandboxUrl : _payUSettings.ProductionUrl;
            


            // Generar la firma (Signature) de forma segura en el backend
            formData.Signature = GenerateSignature(formData);

            return formData;
        }



        public bool ValidateSignature(PayUConfirmationDTO confirmationDTO)
        {
            // Convertir el valor recibido a decimal utilizando CultureInfo.InvariantCulture para forzar el uso de puntos (.)
            if (!decimal.TryParse(confirmationDTO.Value.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value))
            {
                return false;
            }


            // Formatear el valor a F1 y F2 utilizando CultureInfo.InvariantCulture para asegurar que los puntos (.) sean utilizados
            var txValueFormattedF1 = value.ToString("F1", CultureInfo.InvariantCulture); // Con un decimal
            var txValueFormattedF2 = value.ToString("F2", CultureInfo.InvariantCulture); // Con dos decimales

            var signatureStringF1 = $"{_payUSettings.ApiKey}~{confirmationDTO.Merchant_id}~{confirmationDTO.Reference_sale}~{txValueFormattedF1}~{confirmationDTO.Currency}~{confirmationDTO.State_pol}";
            var expectedSignatureF1 = GenerateSignature(signatureStringF1);

            var signatureStringF2 = $"{_payUSettings.ApiKey}~{confirmationDTO.Merchant_id}~{confirmationDTO.Reference_sale}~{txValueFormattedF2}~{confirmationDTO.Currency}~{confirmationDTO.State_pol}";
            var expectedSignatureF2 = GenerateSignature(signatureStringF2);


            // Comparar las firmas generadas con la firma recibida
            return expectedSignatureF1 == confirmationDTO.Sign || expectedSignatureF2 == confirmationDTO.Sign;
        }



        public bool ValidateSignature(PayUResponseDTO responseDTO)
        {
            // Convertir TX_VALUE a decimal utilizando CultureInfo.InvariantCulture para forzar el uso de puntos (.)
            if (!decimal.TryParse(responseDTO.TX_VALUE.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal txValue))
            {
                Console.WriteLine("Error al convertir TX_VALUE a decimal.");
                return false;
            }

            // Aplicar redondeo según las reglas especificadas
            decimal roundedValue = RoundPayUValue(txValue);

            // Generar la cadena de firma con el valor ajustado
            var signatureString = $"{_payUSettings.ApiKey}~{responseDTO.MerchantId}~{responseDTO.ReferenceCode}~{roundedValue.ToString("F1", CultureInfo.InvariantCulture)}~{responseDTO.Currency}~{responseDTO.TransactionState}";
            Console.WriteLine($"Cadena de firma generada: {signatureString}");

            var expectedSignature = GenerateSignature(signatureString);

            // Log para depuración
            Console.WriteLine($"Firma esperada: {expectedSignature}");
            Console.WriteLine($"Firma recibida: {responseDTO.Signature}");

            return expectedSignature == responseDTO.Signature;
        }

        // Método para aplicar las reglas de redondeo de PayU
        private decimal RoundPayUValue(decimal value)
        {
            // Obtener los decimales
            decimal roundedValue = Math.Round(value, 2, MidpointRounding.ToEven); // Redondeo estándar al segundo decimal

            // Extraer el primer y segundo decimal
            string[] valueParts = value.ToString("F2", CultureInfo.InvariantCulture).Split('.');
            if (valueParts.Length == 2)
            {
                int firstDecimal = int.Parse(valueParts[1][0].ToString());
                int secondDecimal = int.Parse(valueParts[1][1].ToString());

                // Aplicar las reglas de PayU:
                if (secondDecimal == 5)
                {
                    if (firstDecimal % 2 == 0)
                    {
                        // Redondear hacia abajo si el primer decimal es par y el segundo es 5
                        roundedValue = Math.Floor(value * 10) / 10;
                    }
                    else
                    {
                        // Redondear hacia arriba si el primer decimal es impar y el segundo es 5
                        roundedValue = Math.Ceiling(value * 10) / 10;
                    }
                }
            }

            return roundedValue;
        }






        private string GenerateSignature(string signatureString)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(signatureString);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
    }
