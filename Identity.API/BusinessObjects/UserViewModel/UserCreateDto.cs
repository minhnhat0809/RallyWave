namespace Identity.API.BusinessObjects.UserViewModel;

public class UserCreateDto
{
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }

    public int PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public sbyte Status { get; set; }

    public string Password { get; set; } = null!;
}