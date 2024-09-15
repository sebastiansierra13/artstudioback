using artstudio.DTOs;
using artstudio.Services;
using Microsoft.Extensions.Options;
using artstudio.Configuration;

public interface IPayUFormService
{
    PayUFormDTO GeneratePaymentForm(PayUFormDTO formData);
    PayUFormDTO CreatePaymentForm(OrderDTO orderDTO); // Método ya existente
}

public class PayUFormService : IPayUFormService
{
    private readonly PayUSettings _payUSettings;
    private readonly IPayUService _payUService;

    public PayUFormService(IOptions<PayUSettings> payUSettings, IPayUService payUService)
    {
        _payUSettings = payUSettings.Value;
        _payUService = payUService;
    }

    public PayUFormDTO CreatePaymentForm(OrderDTO orderDTO)
    {
        var referenceCode = $"Order-{orderDTO.OrderId}";
        var amount = orderDTO.Total;
        var tax = CalculateTax(amount);
        var taxReturnBase = amount - tax;

        // Determina si estás en modo sandbox o producción según la configuración
        var payUUrl = _payUSettings.TestMode ? _payUSettings.SandboxUrl : _payUSettings.ProductionUrl;

        var formData = new PayUFormDTO
        {
            MerchantId = _payUSettings.MerchantId,
            AccountId = _payUSettings.AccountId,
            Description = $"Order {orderDTO.OrderId}",
            ReferenceCode = referenceCode,
            Amount = amount,
            Tax = tax,
            TaxReturnBase = taxReturnBase,
            Currency = "COP",
            BuyerEmail = orderDTO.Email,
            Telephone = orderDTO.Telephone,
            BuyerFullName = orderDTO.BuyerFullName,
            ResponseUrl = _payUSettings.ResponseUrl,
            ConfirmationUrl = _payUSettings.ConfirmationUrl,
            ShippingAddress = orderDTO.ShippingAddress,
            ShippingCity = orderDTO.ShippingCity,
            ShippingCountry = "CO",
            Test = _payUSettings.TestMode ? "1" : "0"
        };

        formData.Signature = _payUService.GenerateSignature(formData);
        return formData;
    }

    public PayUFormDTO GeneratePaymentForm(PayUFormDTO formData)
    {
        formData.Signature = _payUService.GenerateSignature(formData);
        return formData;
    }

    private decimal CalculateTax(decimal amount)
    {
        return amount * 0.19m; // Ejemplo: 19% de IVA
    }

    private decimal CalculateTaxReturnBase(decimal amount)
    {
        return amount - CalculateTax(amount);
    }
}