using System.ComponentModel.DataAnnotations;

namespace BookingManagement.DTOs.BookingDto;

public class BookingCreateDto
{
    public int? UserId { get; set; }
    
    public int? MatchId { get; set; }
    
    [Required]
    public int CourtId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly TimeStart { get; set; }

    [Required]
    public TimeOnly TimeEnd { get; set; }
    
    public string? Note { get; set; }
}