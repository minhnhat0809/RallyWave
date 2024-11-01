namespace BookingManagement.DTOs.BookingDto.ViewDto;

public class BookingFilterDto
{
    public DateOnly? Date { get; set; }   
    public DateOnly? DateFrom { get; set; }    
    public DateOnly? DateTo { get; set; }       
    public TimeOnly? TimeStart { get; set; }
    public TimeOnly? TimeEnd { get; set; }
    public sbyte? Status { get; set; }
}