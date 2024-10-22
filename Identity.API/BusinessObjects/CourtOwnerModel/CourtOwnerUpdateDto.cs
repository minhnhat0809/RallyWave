namespace Identity.API.BusinessObjects.CourtOwnerModel;

public class CourtOwnerUpdateDto
{
        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public int PhoneNumber { get; set; }

        public string Gender { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string Address { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string? Avatar { get; set; }
    
        public string? TaxCode { get; set; }
    
}