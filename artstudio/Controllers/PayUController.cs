using artstudio.DTOs;
using artstudio.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using artstudio.Models;
using System;

namespace artstudio.Controllers
{
    [ApiController]
    [Route("api/payu")]
    public class PayUController : ControllerBase
    {
        private readonly IPayUService _payUService;
        private readonly IPayUFormService _payUFormService;
        private readonly MiDbContext _context;
        private readonly ILogger<PayUController> _logger;

        public PayUController(IPayUService payUService, IPayUFormService payUFormService, MiDbContext context, ILogger<PayUController> logger)
        {
            _payUService = payUService;
            _context = context;
            _logger = logger;
            _payUFormService = payUFormService;
        }

        [HttpPost("createPaymentForm")]
        public IActionResult CreatePaymentForm([FromBody] PayUFormDTO payUFormDTO)
        {
            if (payUFormDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var paymentForm = _payUFormService.GeneratePaymentForm(payUFormDTO);
                return Ok(paymentForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating payment form");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromForm] PayUConfirmationDTO confirmationDTO)
        {
            if (confirmationDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Verificar la firma
                var signature = _payUService.GenerateSignature(new PayUFormDTO
                {
                    MerchantId = confirmationDTO.MerchantId,
                    ReferenceCode = confirmationDTO.ReferenceSale,
                    Amount = confirmationDTO.Amount,
                    Currency = confirmationDTO.Currency
                });

                if (signature != confirmationDTO.Signature)
                {
                    _logger.LogWarning("Invalid signature received for reference: {ReferenceCode}", confirmationDTO.ReferenceSale);
                    return BadRequest("Invalid signature.");
                }

                // Procesar el estado de la transacción
                var order = await _context.Paymenttransactions.SingleOrDefaultAsync(o => o.ReferenceCode == confirmationDTO.ReferenceSale);
                if (order == null)
                {
                    _logger.LogWarning("Order not found for reference: {ReferenceCode}", confirmationDTO.ReferenceSale);
                    return NotFound("Order not found.");
                }

                order.TransactionState = confirmationDTO.TransactionState;
                _context.Paymenttransactions.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment confirmation processed for reference: {ReferenceCode}", confirmationDTO.ReferenceSale);
                return Ok("Confirmation received.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment confirmation");
                return StatusCode(500, "An error occurred while processing the confirmation.");
            }
        }

        [HttpPost("response")]
        public async Task<IActionResult> Response([FromForm] PayUResponseDTO responseDTO)
        {
            if (responseDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var order = await _context.Paymenttransactions.SingleOrDefaultAsync(o => o.ReferenceCode == responseDTO.ReferenceCode);
                if (order == null)
                {
                    _logger.LogWarning("Order not found for reference: {ReferenceCode}", responseDTO.ReferenceCode);
                    return NotFound("Order not found.");
                }

                order.TransactionState = responseDTO.TransactionState;
                _context.Paymenttransactions.Update(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payment response processed for reference: {ReferenceCode}", responseDTO.ReferenceCode);

                // Redirigir al usuario a una página de éxito o error según el estado de la transacción
                if (responseDTO.TransactionState == "APPROVED")
                {
                    return Redirect("https://www.tutienda.com/success");
                }
                else
                {
                    return Redirect("https://www.tutienda.com/failure");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment response");
                return StatusCode(500, "An error occurred while processing the response.");
            }
        }
    }
}