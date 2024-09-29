using BookingManagement.DTOs;

namespace BookingManagement.Service;

public interface IBookingService
{
    Task<ResponseDto> GetBookings(string? filterField,
        string? filterValue,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);
}