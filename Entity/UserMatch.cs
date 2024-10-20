namespace Entity;

public partial class UserMatch
{
    public int UserId { get; set; }

    public int MatchId { get; set; }

    public sbyte? Status { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
