namespace MatchManagement.DTOs.MatchDto.ViewDto;

public class MatchesViewDto (
    int matchId,
    string sportName,
    string matchName,
    string matchType,
    sbyte players,
    sbyte teamSize,
    string? location,
    DateOnly date,
    TimeOnly timeStart,
    TimeOnly timeEnd)
{
    public int MatchId { get; set; } = matchId;

    public string SportName { get; set; } = sportName;

    public string MatchName { get; set; } = matchName;
    
    public string MatchType { get; set; } = matchType;

    public string? Location { get; set; } = location;
    
    public DateOnly Date { get; set; } = date;
    
    public TimeOnly TimeStart { get; set; } = timeStart;
    
    public TimeOnly TimeEnd { get; set; } = timeEnd;
    
    public sbyte Players { get; set; } = players;
    
    public sbyte TeamSize { get; set; } = teamSize;
}