using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement.Repository;

public interface IBookingRepo : IRepositoryBase<Booking>
{
    Task<ResponseListDto<BookingsViewDto>> GetBookings(string? subject, int? subjectId, string filterField, string filterValue, int pageNumber, int pageSize);

    Task<Booking?> GetBookingById(int bookingId);

    Task CreateBooking(Booking booking);

    Task UpdateBooking(Booking booking);

    Task DeleteBooking(Booking booking);
}