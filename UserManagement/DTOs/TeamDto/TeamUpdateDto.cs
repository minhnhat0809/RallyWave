namespace UserManagement.DTOs.TeamDto;

public class TeamUpdateDto
{
    public int TeamId { get; set; }

    public int SportId { get; set; }

    public int CreateBy { get; set; }

    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }

    public sbyte Status { get; set; }
}