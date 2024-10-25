using System.Diagnostics.Contracts;

namespace UserManagement.DTOs.TeamDto;

public class TeamUpdateDto
{
    public required int TeamId { get; set; }
    
    public required int SportId { get; set; }

    public required int CreateBy { get; set; } 

    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }
}