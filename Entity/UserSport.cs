namespace Entity;

public partial class UserSport
{
    public int UserId { get; set; }

    public int SportId { get; set; }

    public sbyte? Level { get; set; }

    public ulong? Status { get; set; }

    public virtual Sport Sport { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
