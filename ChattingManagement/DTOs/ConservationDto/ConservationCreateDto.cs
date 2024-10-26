namespace ChattingManagement.DTOs.ConservationDto;

public class ConservationCreateDto
{
    public int ConservationId { get; set; }

    /*
    public int TeamId { get; set; } // unique

    public int? MatchId { get; set; } // unique
    */

    public string ConservationName { get; set; } = null!;

    public virtual ICollection<UserConservation?> Users { get; set; } = new List<UserConservation?>();
}