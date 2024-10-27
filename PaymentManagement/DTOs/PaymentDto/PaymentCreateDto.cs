using System.ComponentModel.DataAnnotations;

namespace PaymentManagement.DTOs.PaymentDto;

public class PaymentCreateDto()
{
    public int? BookingId { get; set; }

    public PaymentCreateForSubDto? PaymentCreateForSub { get; set; }

    [Required] public string Type { get; set; } = null!;

    [Required] public string SuccessUrl { get; set; } = null!;
    
    [Required] public string CancelUrl { get; set; } = null!;
}