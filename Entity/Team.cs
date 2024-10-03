using System;
using System.Collections.Generic;

namespace Entity;

public partial class Team
{
    public int TeamId { get; set; }

    public int? SportId { get; set; }

    public string TeamName { get; set; } = null!;

    public sbyte? TeamSize { get; set; }

    public sbyte Status { get; set; }

    public virtual Conservation? Conservation { get; set; }

    public virtual Sport? Sport { get; set; }

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
