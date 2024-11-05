namespace UserManagement.DTOs.CourtOwnerDto.ViewDto;

public class CourtOwnerViewDto
{
    public int CourtOwnerId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public int? PhoneNumber { get; set; }

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Address { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string? Avatar { get; set; }

    public sbyte Status { get; set; }

    public DateTime CreatedDate { get; set; }
    public sbyte IsTwoFactorEnabled { get; set; }

    public CourtOwnerViewDto(int courtOwnerId, string? name, string? email, int? phoneNumber, string gender, DateOnly dob, string address, string province, string? avatar, sbyte status)
    {
        CourtOwnerId = courtOwnerId;
        Name = name;
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