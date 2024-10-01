namespace CourtManagement.DTOs.CourtDto.ViewDto;

public class CourtViewDto
{
    public int CourtId { get; set; }

    public int? CourtOwnerId { get; set; }

    public int? SportId { get; set; }

    public string CourtName { get; set; } = null!;

    public sbyte? MaxPlayers { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public sbyte Status { get; set; }
}