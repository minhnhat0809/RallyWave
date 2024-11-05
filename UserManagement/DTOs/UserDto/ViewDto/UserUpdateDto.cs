namespace UserManagement.DTOs.UserDto.ViewDto;

public class UserUpdateDto
{
    
    public ICollection<UserSportUpdateDto>? UserSports { get; set; } = new List<UserSportUpdateDto>();
}

public class UserSportUpdateDto
{
    public int SportId { get; set; }
    public sbyte SportLevel { get; set; }
}