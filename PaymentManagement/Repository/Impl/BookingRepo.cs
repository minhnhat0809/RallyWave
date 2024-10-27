using Entity;

namespace PaymentManagement.Repository.Impl;

public class BookingRepo(RallyWaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    
}