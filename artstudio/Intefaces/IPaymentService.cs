// Interfaces/IPaymentService.cs
using artstudio.DTOs;
public interface IPaymentService
{
    Task<PaymentInfo> InitiatePayment(PaymentRequest request);
    Task<PaymentResult> ConfirmPayment(PaymentConfirmation confirmation);
}