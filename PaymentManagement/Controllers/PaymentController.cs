using Microsoft.AspNetCore.Mvc;
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
    }
}
