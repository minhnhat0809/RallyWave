namespace Identity.API.BusinessObjects.CourtOwnerModel;

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

}