using WebApplication1.Repository;

namespace BookingManagement.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo bookingRepo { get; }
}