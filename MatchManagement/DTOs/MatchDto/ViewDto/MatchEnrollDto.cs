namespace MatchManagement.DTOs.MatchDto.ViewDto;

public class MatchEnrollDto(
    int userId,
    int sportId,
    TimeOnly timeStart,
    TimeOnly timeEnd,
    DateOnly date,
    sbyte mode,
    string? gender,
    sbyte? minAge,
    sbyte? maxAge,
    sbyte? minLevel,
    sbyte? maxLevel)
{
    public int UserId { get; set; } = userId;
    public int SportId { get; set; } = sportId;
    public TimeOnly TimeStart { get; set; } = timeStart;

    public TimeOnly TimeEnd { get; set; } = timeEnd;

    public DateOnly Date { get; set; } = date;

    public sbyte Mode { get; set; } = mode;

    public string? Gender { get; set; } = gender;
    
    public sbyte? MinAge { get; set; } = minAge;

    public sbyte? MaxAge { get; set; } = maxAge;

    public sbyte? MinLevel { get; set; } = minLevel;

    public sbyte? MaxLevel { get; set; } = maxLevel;
}