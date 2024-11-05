namespace Identity.API.BusinessObjects.SportUserDto;

public class SportUserViewDto
{
    public int SportId { get; set; }

    public sbyte? Level { get; set; }

    public ulong? Status { get; set; }

    public virtual SportViewDto Sport { get; set; } = null!;
}

public class SportViewDto
{
    public int SportId { get; set; }

    public string SportName { get; set; } = null!;
}