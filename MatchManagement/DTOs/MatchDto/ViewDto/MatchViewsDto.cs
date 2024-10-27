namespace MatchManagement.DTOs.MatchDto.ViewDto;

public class MatchViewsDto(
    int matchId,
    string sportName,
    string matchName,
    int createById,
    string createBy,
    sbyte matchType,
    sbyte teamSize,
    sbyte? minLevel,
    sbyte? maxLevel,
    DateOnly date,
    TimeOnly timeStart,
    TimeOnly timeEnd,
    string location,
    sbyte status)
{
    public int MatchId { get; set; } = matchId;

    public string SportName { get; set; } = sportName;

    public string MatchName { get; set; } = matchName;

    public int CreateById { get; set; } = createById;

    public string CreateBy { get; set; } = createBy;
    public sbyte MatchType { get; set; } = matchType;

    public sbyte TeamSize { get; set; } = teamSize;

    public sbyte? MinLevel { get; set; } = minLevel;

    public sbyte? MaxLevel { get; set; } = maxLevel;

    public DateOnly Date { get; set; } = date;

    public TimeOnly TimeStart { get; set; } = timeStart;

    public TimeOnly TimeEnd { get; set; } = timeEnd;

    public string Location { get; set; } = location;

    public sbyte Status { get; set; } = status;
}