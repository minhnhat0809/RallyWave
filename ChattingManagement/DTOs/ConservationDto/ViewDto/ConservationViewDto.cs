namespace ChattingManagement.DTOs.ConservationDto.ViewDto;

public class ConservationViewDto
{
    public int ConservationId { get; set; }
    
    public int? MatchId { get; set; }

    public string ConservationName { get; set; } = null!;

    public sbyte Status { get; set; }
}