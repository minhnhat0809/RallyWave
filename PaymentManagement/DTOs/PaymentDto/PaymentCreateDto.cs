namespace PaymentManagement.DTOs.PaymentDto;

public class PaymentCreateDto()
{
    public int? BookingId { get; set; }

    public PaymentCreateForSubDto? PaymentCreateForSub { get; set; }
}