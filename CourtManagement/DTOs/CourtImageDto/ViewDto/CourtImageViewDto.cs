namespace CourtManagement.DTOs.CourtImageDto.ViewDto;

public class CourtImageViewDto(int imageId, string imageUrl)
{
    public int ImageId { get; set; } = imageId;

    public string ImageUrl { get; set; } = imageUrl;
}