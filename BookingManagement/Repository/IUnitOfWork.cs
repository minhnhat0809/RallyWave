namespace WebApplication1.Repository;

public interface IUnitOfWork
{ 
    IBookingRepo bookingRepo { get; }
}