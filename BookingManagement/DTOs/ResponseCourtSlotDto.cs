namespace BookingManagement.DTOs;

public class ResponseCourtSlotDto(double? cost, string? message, bool isSucceed, int statusCode)
{
    public double? Cost { get; set; } = cost;
    public string? Message { get; set; } = message;
    public bool IsSucceed { get; set; } = isSucceed;
    public int StatusCode { get; set; } = statusCode;
}