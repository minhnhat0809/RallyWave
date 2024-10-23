using Entity;

namespace BookingManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IBookingRepo BookingRepo { get; } = new BookingRepo(context);

    public ICourtRepo CourtRepo { get; } = new CourtRepo(context);

    public ISlotRepo SlotRepo { get; } = new SlotRepo(context);

    public IMatchRepo MatchRepo { get; } = new MatchRepo(context);

    public IUserRepo UserRepo { get; } = new UserRepo(context);
}