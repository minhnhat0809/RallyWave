namespace BookingManagement.DTOs.BookingDto;

public class BookingCreateDto
{
    public int? UserId { get; set; }

    public int? MatchId { get; set; }

    public int? CourtId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }
    
    public string? Note { get; set; }
}