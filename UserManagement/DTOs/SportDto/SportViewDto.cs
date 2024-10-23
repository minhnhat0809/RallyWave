namespace UserManagement.DTOs.SportDto;

public class SportViewDto
{
    public int SportId { get; set; }

    public string SportName { get; set; } = null!;

    public ulong Status { get; set; }
}