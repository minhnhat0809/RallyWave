using BookingManagement.Repository;

namespace BookingManagement.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo bookingRepo { get; }
}