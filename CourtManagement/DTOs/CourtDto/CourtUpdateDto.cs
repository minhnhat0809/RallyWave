using System.ComponentModel.DataAnnotations;
namespace CourtManagement.DTOs.CourtDto;

public class CourtUpdateDto
{
    [Required]
    public int SportId { get; set; }

    [Required(ErrorMessage = "Court name is required.")]
    [MinLength(1, ErrorMessage = "Court name cannot be empty.")]
    public string CourtName { get; set; } = null!;

    public sbyte? MaxPlayers { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [MinLength(1, ErrorMessage = "Address cannot be empty.")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "Province is required.")]
    [MinLength(1, ErrorMessage = "Province cannot be empty.")]
    public string Province { get; set; } = null!;

    [Required(ErrorMessage = "Status is required.")]
    [MinLength(1, ErrorMessage = "Status cannot be empty.")] 
    public string Status { get; set; } = null!;
}