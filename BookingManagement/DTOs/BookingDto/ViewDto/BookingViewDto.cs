using BookingManagement.DTOs.CourtDto.ViewDto;
using BookingManagement.DTOs.MatchDto.ViewDto;
using BookingManagement.DTOs.UserDto.ViewDto;

namespace BookingManagement.DTOs.BookingDto.ViewDto;

public class BookingViewDto
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int? MatchId { get; set; }

    public int? CourtId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public DateTime CreateAt { get; set; }

    public string? Note { get; set; }

    public sbyte Status { get; set; }
    
    public CourtViewDto? Court { get; set; }

    public MatchViewDto? Match { get; set; }

    public UserViewDto? User { get; set; }
}