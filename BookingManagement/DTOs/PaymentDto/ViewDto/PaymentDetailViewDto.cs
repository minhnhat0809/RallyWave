namespace BookingManagement.DTOs.PaymentDto.ViewDto;

public class PaymentDetailViewDto
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public string? Note { get; set; }

    public double Total { get; set; }

    public string Type { get; set; } = null!;

    public sbyte Status { get; set; }
}