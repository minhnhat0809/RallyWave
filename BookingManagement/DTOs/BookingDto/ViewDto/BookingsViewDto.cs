using BookingManagement.DTOs.PaymentDto.ViewDto;
using Entity;

namespace BookingManagement.DTOs.BookingDto.ViewDto;

public class BookingsViewDto
{
    public int BookingId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public sbyte Status { get; set; }
    
    public PaymentDetail? PaymentDetail { get; set; }

    public BookingsViewDto(int bookingId, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd, sbyte status, PaymentDetail? paymentDetail)
    {
        BookingId = bookingId;
        Date = date;
        TimeStart = timeStart;
        TimeEnd = timeEnd;
        Status = status;
        PaymentDetail = paymentDetail;
    }
}