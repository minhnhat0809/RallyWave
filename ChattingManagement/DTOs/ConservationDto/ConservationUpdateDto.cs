namespace ChattingManagement.DTOs.ConservationDto;

public class ConservationUpdateDto
{
    public int ConservationId { get; set; }

    public string ConservationName { get; set; } = null!;

    public virtual ICollection<UserConservation?> Users { get; set; } = new List<UserConservation?>();
}

public class UserConservation{
    public int UserId { get; set; }
}