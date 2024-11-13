namespace MatchManagement.DTOs.MatchDto;

public class MatchFilterDto
{
    public string? MatchName { get; set; }
    
    public sbyte? TeamSize { get; set; }
    
    public sbyte? MinLevel { get; set; }
    
    public sbyte? MaxLevel { get; set; }
    
    public sbyte? Mode { get; set; }
    
    public sbyte? MinAge { get; set; }
    
    public sbyte? MaxAge { get; set; }
    
    public string? Gender { get; set; }
    
    public DateOnly? Date { get; set; }
    
    public DateOnly? DateFrom { get; set; }
    
    public DateOnly? DateTo { get; set; }
    
    public TimeOnly? TimeStart { get; set; }
    
    public TimeOnly? TimeEnd { get; set; }
    
    public sbyte? MatchType { get; set; }
    
    public sbyte? Status { get; set; }
}
