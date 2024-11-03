namespace PaymentManagement.Repository;

public interface IUnitOfWork
{
    IPaymentRepo PaymentRepo { get; }
    
    IBookingRepo BookingRepo { get; }
    
    IUserRepo UserRepo { get; }
    
    ISubscriptionRepo SubscriptionRepo { get; }
    
    ICourtOwnerRepo CourtOwnerRepo { get; }
    
    IMatchRepo MatchRepo { get; }
    
    ICourtRepo CourtRepo { get; }
}