namespace BookingManagement.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo BookingRepo { get; }
    
    ICourtRepo CourtRepo { get; }
    
    ISlotRepo SlotRepo { get; }
    
    IMatchRepo MatchRepo { get; }
    
    IUserRepo UserRepo { get; }
}