namespace PaymentManagement.DTOs.PaymentDto.ViewDto;

public class PaymentViewDto
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public int? UserId { get; set; }

    public int? CourtOwnerId { get; set; }

    public int? SubId { get; set; }

    public string? Note { get; set; }

    public double Total { get; set; }

    public string Type { get; set; } = null!;

    public sbyte Status { get; set; }
}