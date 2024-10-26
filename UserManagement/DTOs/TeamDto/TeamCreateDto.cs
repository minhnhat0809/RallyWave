using Entity;

namespace UserManagement.DTOs.TeamDto;

public class TeamCreateDto
{
    public required int SportId { get; set; }

    public required int CreateBy { get; set; } 

    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }
    
    public virtual ICollection<UserCreateTeamDto> UserTeams { get; set; } = new List<UserCreateTeamDto>();
}

public class UserCreateTeamDto
{ 
    public int UserId { get; set; }
}