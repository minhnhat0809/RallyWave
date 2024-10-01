using CourtManagement.Repository;

namespace CourtManagement.Repository;

public interface IUnitOfWork
{
    ICourtRepo courtRepo { get; }
}