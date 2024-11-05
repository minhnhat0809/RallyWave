using System.ComponentModel.DataAnnotations;
using Entity;
using UserManagement.DTOs.SportUserDto;

namespace UserManagement.DTOs.UserDto.ViewDto;

public class UserViewDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public int PhoneNumber { get; set; } // You may want to switch this back to `string`
    public string Gender { get; set; } = null!;
    public DateOnly Dob { get; set; }
    public string Address { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string? Avatar { get; set; }
    public sbyte Status { get; set; }
    
    public virtual ICollection<UserSportViewDto> UserSports { get; set; } = new List<UserSportViewDto>();
    public UserViewDto(int userId, string userName, string? email, int phoneNumber, string gender, DateOnly dob, string address, string province, string? avatar, sbyte status)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        PhoneNumber = phoneNumber;
        Gender = gender;
        Dob = dob;
        Address = address;
        Province = province;
        Avatar = avatar;
        Status = status;
    }

}

