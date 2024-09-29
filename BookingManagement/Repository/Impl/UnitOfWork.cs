using Entity;

namespace WebApplication1.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    private readonly RallywaveContext _context = context;
    
    public IBookingRepo bookingRepo { get; } = new BookingRepo(context);
}