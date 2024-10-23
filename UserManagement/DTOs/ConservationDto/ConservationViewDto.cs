namespace UserManagement.DTOs.ConservationDto;

public class ConservationViewDto
{
    public int ConservationId { get; set; }

    public int? TeamId { get; set; }

    public int? MatchId { get; set; }

    public string ConservationName { get; set; } = null!;

    public sbyte Status { get; set; }
}