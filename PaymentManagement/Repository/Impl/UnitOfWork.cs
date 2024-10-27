using Entity;

namespace PaymentManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IPaymentRepo PaymentRepo { get; } = new PaymentRepo(context);
    
    public IBookingRepo BookingRepo { get; } = new BookingRepo(context);

    public IUserRepo UserRepo { get; } = new UserRepo(context);

    public ISubscriptionRepo SubscriptionRepo { get; } = new SubscriptionRepo(context);

    public ICourtOwnerRepo CourtOwnerRepo { get; } = new CourtOwnerRepo(context);

    public IMatchRepo MatchRepo { get; } = new MatchRepo(context);

    public ICourtRepo CourtRepo { get; } = new CourtRepo(context);
}