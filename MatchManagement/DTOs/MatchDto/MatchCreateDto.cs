using System.ComponentModel.DataAnnotations;

namespace MatchManagement.DTOs.MatchDto;

public class MatchCreateDto
{
    [Required(ErrorMessage = "Sport ID is required.")]
    public int SportId { get; set; }

    [Required(ErrorMessage = "Match name is required.")]
    [StringLength(100, ErrorMessage = "Match name cannot exceed 100 characters.")]
    public string MatchName { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
    public string? Note { get; set; }

    [Required(ErrorMessage = "Match type is required.")]
    [Range(0, 4, ErrorMessage = "Match type must be between 0 and 10.")]
    public sbyte MatchType { get; set; }

    [Required(ErrorMessage = "Team size is required.")]
    [Range(1, 20, ErrorMessage = "Team size must be between 1 and 20.")]
    public sbyte TeamSize { get; set; }

    [Range(1, 5, ErrorMessage = "Minimum level must be between 1 and 5.")]
    public sbyte? MinLevel { get; set; }

    [Range(1, 5, ErrorMessage = "Maximum level must be between 1 and 5.")]
    public sbyte? MaxLevel { get; set; }

    [Required(ErrorMessage = "Date is required.")]
    public DateOnly Date { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    public TimeOnly TimeStart { get; set; }

    [Required(ErrorMessage = "End time is required.")]
    public TimeOnly TimeEnd { get; set; }

    [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
    public string? Location { get; set; }

    [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
    public string? Gender { get; set; }

    [Range(9, 70, ErrorMessage = "Age range must be between 9 and 70.")]
    public sbyte? MinAge { get; set; }
    
    [Range(9, 70, ErrorMessage = "Age range must be between 9 and 70.")]
    public sbyte? MaxAge { get; set; }

    [Range(0, 10, ErrorMessage = "Iteration must be between 0 and 10.")]
    public sbyte? Iteration { get; set; }

    [Range(2, 12, ErrorMessage = "Blocking Off must be between 2 or 12.")]
    public sbyte? BlockingOff { get; set; }

    public ulong? AutoApprove { get; set; }

    public ulong? AddByOthers { get; set; }

    public ulong? Notification { get; set; }

    [Required(ErrorMessage = "Mode is required.")]
    public sbyte Mode { get; set; }
    
}