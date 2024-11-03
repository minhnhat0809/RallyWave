namespace MatchManagement.DTOs.UserDto.ViewDto;

public class UserMatchDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;
    
    public string? Avatar { get; set; }
}