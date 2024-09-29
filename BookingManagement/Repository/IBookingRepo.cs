using Entity;

namespace WebApplication1.Repository;

public interface IBookingRepo : IRepositoryBase<Booking>
{
    Task<List<Booking>> GetBookings(string? filterField, string? filterValue);
}