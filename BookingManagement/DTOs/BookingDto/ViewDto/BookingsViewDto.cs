namespace BookingManagement.DTOs.BookingDto.ViewDto;

public class BookingsViewDto(
    int bookingId,
    DateOnly date,
    TimeOnly timeStart,
    TimeOnly timeEnd,
    double cost,
    sbyte status)
{
    public int BookingId { get; set; } = bookingId;

    public DateOnly Date { get; set; } = date;

    public TimeOnly TimeStart { get; set; } = timeStart;

    public TimeOnly TimeEnd { get; set; } = timeEnd;

    public double Cost { get; set; } = cost;

    public sbyte Status { get; set; } = status;
}