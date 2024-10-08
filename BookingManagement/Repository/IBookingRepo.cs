using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement.Repository;

public interface IBookingRepo : IRepositoryBase<Booking>
{
    Task<List<BookingsViewDto>> GetBookings(string? filterField, string? filterValue);

    Task<Booking?> GetBookingById(int bookingId);

    Task CreateBooking(Booking booking);

    Task UpdateBooking(Booking booking);

    Task DeleteBooking(Booking booking);
}