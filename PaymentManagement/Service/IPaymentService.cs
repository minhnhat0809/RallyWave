﻿using Net.payOS.Types;
using PaymentManagement.DTOs;
using PaymentManagement.DTOs.PaymentDto;

namespace PaymentManagement.Service;

public interface IPaymentService
{
    Task<ResponseDto> GetPayments();

    Task<ResponseDto> GetPaymentById(int id);

    Task<ResponseDto> ProcessPayment(PaymentCreateDto paymentCreateDto);

    Task<ResponseDto> HandlePayment(WebhookType webhookType);
    
    Task<ResponseDto> ConfirmWebHook(string url);
}