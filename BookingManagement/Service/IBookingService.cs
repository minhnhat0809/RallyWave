using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;

namespace BookingManagement.Service;

public interface IBookingService
{
    Task<ResponseDto> GetBookings(string? filterField,
        string? filterValue,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);

    Task<ResponseDto> GetBookingById(int bookingId);

    Task<ResponseDto> CreateBooking(BookingCreateDto bookingCreateDto);

    Task<ResponseDto> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto);

    Task<ResponseDto> DeleteBooking(int id);
}