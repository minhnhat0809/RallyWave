using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;

namespace BookingManagement.Service;

public interface IBookingService
{
    Task<ResponseDto> GetBookings(
        string? subject,
        int? subjectId,
        BookingFilterDto? bookingFilterDto,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);

    Task<ResponseDto> GetBookingById(int bookingId);

    Task<ResponseDto> CreateBooking(BookingCreateDto bookingCreateDto);

    Task<ResponseDto> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto);

    Task<ResponseDto> DeleteBooking(int id);
}