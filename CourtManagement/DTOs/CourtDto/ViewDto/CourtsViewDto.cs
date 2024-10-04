namespace CourtManagement.DTOs.CourtDto.ViewDto;

public class CourtsViewDto(
    int courtId,
    string courtName,
    string address,
    string province,
    sbyte status,
    string sportName)
{
    public int CourtId { get; set; } = courtId;

    public string CourtName { get; set; } = courtName;

    public string Address { get; set; } = address;

    public string Province { get; set; } = province;

    public sbyte Status { get; set; } = status;

    public string SportName { get; set; } = sportName;
}