namespace BookingManagement.DTOs.MatchDto.ViewDto;

public class MatchViewDto
{
    public int MatchId { get; set; }

    public int? SportId { get; set; }

    public string MatchName { get; set; } = null!;

    public string? Note { get; set; }

    public sbyte MatchType { get; set; }

    public sbyte TeamSize { get; set; }

    public sbyte? MinLevel { get; set; }

    public sbyte? MaxLevel { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public string? Location { get; set; }

    public string? Gender { get; set; }

    public sbyte? AgeRange { get; set; }

    public sbyte? Iteration { get; set; }

    public sbyte? BlockingOff { get; set; }

    public ulong? AutoApprove { get; set; }

    public ulong? AddByOthers { get; set; }

    public ulong? Notification { get; set; }

    public sbyte Mode { get; set; }

    public sbyte? Status { get; set; } 
}