using PaymentManagement.DTOs;

namespace PaymentManagement.Service;

public interface IPaymentService
{
    Task<ResponseDto> GetPayments();

    Task<ResponseDto> GetPaymentById(int id);

    Task<ResponseDto> ProcessPayment();
}