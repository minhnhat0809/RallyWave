using CourtManagement.DTOs.CourtImageDto.ViewDto;

namespace CourtManagement.DTOs.CourtDto.ViewDto;

public class CourtViewDto(
    int courtId,
    string ownerName,
    string sportName,
    string courtName,
    sbyte? maxPlayers,
    string address,
    string province,
    sbyte status,
    List<CourtImageViewDto>? courtImages
    )
{
    public int CourtId { get; set; } = courtId;

    public string OwnerName { get; set; } = ownerName;

    public string SportName { get; set; } = sportName;

    public string CourtName { get; set; } = courtName!;

    public sbyte? MaxPlayers { get; set; } = maxPlayers;

    public string Address { get; set; } = address;

    public string Province { get; set; } = province;

    public sbyte Status { get; set; } = status;

    public List<CourtImageViewDto>? Images { get; set; } = courtImages;
}