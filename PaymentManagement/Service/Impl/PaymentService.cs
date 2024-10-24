using PaymentManagement.DTOs;

namespace PaymentManagement.Service.Impl;

public class PaymentService : IPaymentService
{
    public async Task<ResponseDto> GetPayments()
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {

        }
        catch (Exception e)
        {
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return response;
    }

    public async Task<ResponseDto> GetPaymentById(int id)
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {

        }
        catch (Exception e)
        {
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return response;
    }

    public async Task<ResponseDto> ProcessPayment()
    {
        var response = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {

        }
        catch (Exception e)
        {
            response.IsSucceed = false;
            response.Message = e.Message;
            response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return response;
    }
}