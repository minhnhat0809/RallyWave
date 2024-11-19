using System.ComponentModel.DataAnnotations;
using BookingManagement.DTOs.Validation;

namespace BookingManagement.DTOs.BookingDto;

public class BookingCreateDto
{
    public int? UserId { get; set; }
    
    public int? MatchId { get; set; }
    
    [Required]
    public int CourtId { get; set; }
    
    [Required]
    [MinDate]
    public DateOnly Date { get; set; }

    [Required]
    [HalfHourOnly]
    public TimeOnly TimeStart { get; set; }

    [Required]
    [HalfHourOnly]
    [EndTime("TimeStart")]
    public TimeOnly TimeEnd { get; set; }
    
    public string? Note { get; set; }
}