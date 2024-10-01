using BookingManagement.Repository;
using Entity;

namespace BookingManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    private readonly RallywaveContext _context = context;
    
    public IBookingRepo bookingRepo { get; } = new BookingRepo(context);

    public ICourtRepo courtRepo { get; } = new CourtRepo(context);

    public ISlotRepo slotRepo { get; } = new SlotRepo(context);

    public IMatchRepo matchRepo { get; } = new MatchRepo(context);
}