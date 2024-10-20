namespace Entity;

public partial class Match
{
    public int MatchId { get; set; }

    public int SportId { get; set; }

    public int CreateBy { get; set; }

    public string MatchName { get; set; } = null!;

    public string? Note { get; set; }

    public sbyte MatchType { get; set; }

    public sbyte TeamSize { get; set; }

    public sbyte? MinLevel { get; set; }

    public sbyte? MaxLevel { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly TimeStart { get; set; }

    public TimeOnly TimeEnd { get; set; }

    public string? Location { get; set; }

    public string? Gender { get; set; }

    public sbyte? MinAge { get; set; }

    public sbyte? MaxAge { get; set; }

    public sbyte? Iteration { get; set; }

    public sbyte? BlockingOff { get; set; }

    public ulong? AutoApprove { get; set; }

    public ulong? AddByOthers { get; set; }

    public ulong? Notification { get; set; }

    public sbyte Mode { get; set; }

    public sbyte? Status { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Conservation? Conservation { get; set; }

    public virtual User CreateByNavigation { get; set; } = null!;

    public virtual Sport Sport { get; set; } = null!;

    public virtual ICollection<UserMatch> UserMatches { get; set; } = new List<UserMatch>();
}
