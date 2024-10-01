using BookingManagement.Repository;
using BookingManagement.Repository.Impl;

namespace BookingManagement.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo bookingRepo { get; }
    
    ICourtRepo courtRepo { get; }
    
    ISlotRepo slotRepo { get; }
    
    IMatchRepo matchRepo { get; }
}