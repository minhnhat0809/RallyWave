using CourtManagement.Repository;

namespace CourtManagement.Repository;

public interface IUnitOfWork
{
    ICourtRepo CourtRepo { get; }
    
    ICourtOwnerRepo CourtOwnerRepo { get; }
    
    ISportRepo SportRepo { get; }
}