using AutoMapper;
using PaymentManagement.DTOs;
using PaymentManagement.DTOs.PaymentDto;
using PaymentManagement.Repository;

namespace PaymentManagement.Service.Impl;

public class PaymentService(IMapper mapper, IUnitOfWork unitOfWork) : IPaymentService
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
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

    public async Task<ResponseDto> ProcessPayment(PaymentCreateDto paymentCreateDto)
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