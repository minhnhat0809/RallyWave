using Entity;

namespace BookingManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    public IBookingRepo bookingRepo { get; } = new BookingRepo(context);

    public ICourtRepo courtRepo { get; } = new CourtRepo(context);

    public ISlotRepo slotRepo { get; } = new SlotRepo(context);

    public IMatchRepo matchRepo { get; } = new MatchRepo(context);
}