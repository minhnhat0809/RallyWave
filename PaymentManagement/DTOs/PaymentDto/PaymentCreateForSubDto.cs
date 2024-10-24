using System.ComponentModel.DataAnnotations;

namespace PaymentManagement.DTOs.PaymentDto;

public class PaymentCreateForSubDto()
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int SubId { get; set; }
}