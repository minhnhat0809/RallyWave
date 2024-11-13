using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using PaymentManagement.DTOs;
using PaymentManagement.DTOs.PaymentDto;
using PaymentManagement.Service;

namespace PaymentManagement.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController(IPaymentService paymentService) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        
        
        
        [HttpPost]
        public async Task<ResponseDto> CheckOut([FromBody] PaymentCreateDto paymentCreateDto)
        {
            var response = await _paymentService.ProcessPayment(paymentCreateDto);
            
            return response;
        }
        
        
        [HttpPost("pay-os/handler")]
        public async Task<ResponseDto> HandlePayment(WebhookType webhookType)
        {
            var response = await _paymentService.HandlePayment(webhookType);
            
            return response;
        }

        [HttpPost("confirm-webhook")]
        public async Task<ResponseDto> ConfirmWebhook([FromBody] string url)
        {
            var response = await _paymentService.ConfirmWebHook(url);

            return response;
        }
    }
}
