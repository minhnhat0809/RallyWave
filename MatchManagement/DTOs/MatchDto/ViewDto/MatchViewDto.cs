namespace MatchManagement.DTOs.MatchDto.ViewDto;

public class MatchViewDto
{
    public int MatchId { get; set; }
    
    public int SportId { get; set; }
    public string SportName { get; set; } = null!;
    
    public int CreateBy { get; set; }

    public string MatchName { get; set; } = null!;

    public string? Note { get; set; }
    
    public string MatchType { get; set; } = "";

    public sbyte TeamSize { get; set; }
    
    public string? MinLevel { get; set; }
    
    public string? MaxLevel { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public string? Location { get; set; }

    public string? Gender { get; set; }

    public sbyte? MinAge { get; set; }

    public sbyte? MaxAge { get; set; }

    public sbyte? Iteration { get; set; }

    public sbyte? BlockingOff { get; set; }

    public ulong? AutoApprove { get; set; }

    public ulong? AddByOthers { get; set; }

    public ulong? Notification { get; set; }
    
    public string Mode { get; set; } = "";
    
    public string? Status { get; set; } = "";
    
    public List<UserDto.ViewDto.UserMatchDto>  Users { get; set; } = [];
}