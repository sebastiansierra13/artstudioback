using artstudio.Configuration;
using artstudio.DTOs;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace artstudio.Services
{
    public interface IPayUService
    {
        string GenerateSignature(PayUFormDTO formData); // Método ya existente
    }

    public class PayUService : IPayUService
    {
        private readonly PayUSettings _payUSettings;

        public PayUService(IOptions<PayUSettings> payUSettings)
        {
            _payUSettings = payUSettings.Value;
        }

        public string GenerateSignature(PayUFormDTO formData)
        {
            var signatureString = $"{_payUSettings.ApiKey}~{formData.MerchantId}~{formData.ReferenceCode}~{formData.Amount}~{formData.Currency}";

            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(signatureString);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }

}
