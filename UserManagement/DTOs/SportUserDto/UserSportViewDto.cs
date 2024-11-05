using UserManagement.DTOs.SportDto;

namespace UserManagement.DTOs.SportUserDto;

public class UserSportViewDto
{
    public int UserId { get; set; }

    public int SportId { get; set; }

    public sbyte? Level { get; set; }

    public ulong? Status { get; set; }

    public virtual SportViewDto Sport { get; set; } = null!;
}


