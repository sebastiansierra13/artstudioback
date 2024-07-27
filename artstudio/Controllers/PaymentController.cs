using Microsoft.AspNetCore.Mvc;
using artstudio.Models;
using Microsoft.EntityFrameworkCore;
using artstudio.DTOs;

namespace artstudio.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
        {
            var paymentInfo = await _paymentService.InitiatePayment(request);
            return Ok(paymentInfo);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmation confirmation)
        {
            var result = await _paymentService.ConfirmPayment(confirmation);
            return Ok(result);
        }
    }

}