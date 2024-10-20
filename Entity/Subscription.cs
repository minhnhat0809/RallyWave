namespace Entity;

public partial class Subscription
{
    public int SubId { get; set; }

    public string SubName { get; set; } = null!;

    public string SubDescription { get; set; } = null!;

    public double Price { get; set; }

    public ulong? IsActive { get; set; }

    public virtual ICollection<CourtOwner> CourtOwners { get; set; } = new List<CourtOwner>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
