namespace Identity.API.BusinessObjects.UserViewModel;

public class UserViewDto
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Email { get; set; }

    public int PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }
    public DateTime CreatedDate { get; set; }

    public UserViewDto(int userId, string userName, string? email, int phoneNumber, string gender, DateOnly dob, string address, string province, string? avatar, sbyte status, DateTime createdDate)
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
        CreatedDate = createdDate;
    }
}