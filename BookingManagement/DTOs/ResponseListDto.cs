namespace BookingManagement.DTOs;

public class ResponseListDto<T> (List<T> data, int totalCount)
{
    public List<T> Data { get; set; } = data;

    public int TotalCount { get; set; } = totalCount;
}