using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs.UserDto.ViewDto;

public class UserViewDto
{
    [Microsoft.Build.Framework.Required]
    public int UserId { get; set; }

    [Microsoft.Build.Framework.Required]
    public string UserName { get; set; } = null!;

    [EmailAddress]
    public string? Email { get; set; }

    [Microsoft.Build.Framework.Required]
    public int PhoneNumber { get; set; }

    [Microsoft.Build.Framework.Required]
    public string Gender { get; set; } = null!;

    [Microsoft.Build.Framework.Required]
    public DateOnly Dob { get; set; }

    [Microsoft.Build.Framework.Required]
    public string Address { get; set; } = null!;

    [Microsoft.Build.Framework.Required]
    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    [Range(0, 1)] // Assuming Status is a binary value (active/inactive)
    public sbyte Status { get; set; }

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
