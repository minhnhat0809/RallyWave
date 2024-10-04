namespace BookingManagement.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo bookingRepo { get; }
    
    ICourtRepo courtRepo { get; }
    
    ISlotRepo slotRepo { get; }
    
    IMatchRepo matchRepo { get; }
    
    IUserRepo userRepo { get; }
}