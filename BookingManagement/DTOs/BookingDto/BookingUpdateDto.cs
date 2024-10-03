using System.ComponentModel.DataAnnotations;

namespace BookingManagement.DTOs.BookingDto;

public class BookingUpdateDto
{
    public int? UserId { get; set; }
    
    public int? MatchId { get; set; }
    
    public int? CourtId { get; set; }
    public int? SlotId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly TimeStart { get; set; }

    [Required]
    public TimeOnly TimeEnd { get; set; }
    
    public string? Note { get; set; }

    [Required]
    public string Status { get; set; } = null!;
}