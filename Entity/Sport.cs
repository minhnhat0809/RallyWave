namespace Entity;

public partial class Sport
{
    public int SportId { get; set; }

    public string SportName { get; set; } = null!;

    public ulong Status { get; set; }

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();
}
