namespace Entity;

public partial class UserTeam
{
    public int UserId { get; set; }

    public int TeamId { get; set; }

    public ulong? Status { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
