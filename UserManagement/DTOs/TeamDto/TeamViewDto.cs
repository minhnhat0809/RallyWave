using Entity;
using UserManagement.DTOs.ConservationDto;
using UserManagement.DTOs.SportDto;
using UserManagement.DTOs.UserTeamDto;

namespace UserManagement.DTOs.TeamDto;

public class TeamViewDto
{
    public int TeamId { get; set; }

    public int CreateBy { get; set; }
    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }

    public sbyte Status { get; set; }
    
    public virtual SportViewDto? Sport { get; set; }
    public virtual ConservationViewDto? Conservation { get; set; }
    public virtual ICollection<UserTeamViewDto> UserTeams { get; set; } = new List<UserTeamViewDto>();
}