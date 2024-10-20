namespace Entity;

public partial class Friendship
{
    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public int? Level { get; set; }

    public virtual User User1 { get; set; } = null!;

    public virtual User User2 { get; set; } = null!;
}
