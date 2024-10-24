using System;
using System.Collections.Generic;

namespace Entity;

public partial class Team
{
    public int TeamId { get; set; }

    public int SportId { get; set; }

    public int CreateBy { get; set; }

    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }

    public sbyte Status { get; set; }

    public virtual Conservation? Conservation { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Sport? Sport { get; set; } = null!;

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
