using System.ComponentModel.DataAnnotations;

namespace CourtManagement.DTOs.CourtDto
{
    public class CourtCreateDto
    {
        [Required]
        public int CourtOwnerId { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required(ErrorMessage = "Court name is required.")]
        [MinLength(1, ErrorMessage = "Court name cannot be empty.")]
        [MaxLength(50, ErrorMessage = "Court name must be less than 50 characters.")]
        public string CourtName { get; set; } = null!;

        public sbyte? MaxPlayers { get; set; }

        public IFormFileCollection? Images { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MinLength(1, ErrorMessage = "Address cannot be empty.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Province is required.")]
        [MinLength(1, ErrorMessage = "Province cannot be empty.")]
        [MaxLength(50, ErrorMessage = "Province must be less than 50 characters.")]
        public string Province { get; set; } = null!;
        
        [Required]
        public sbyte Status { get; set; }
    }
}