namespace Entity;

public partial class Conservation
{
    public int ConservationId { get; set; }

    public int? TeamId { get; set; }

    public int? MatchId { get; set; }

    public string ConservationName { get; set; } = null!;

    public sbyte Status { get; set; }

    public virtual Match? Match { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Team? Team { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
