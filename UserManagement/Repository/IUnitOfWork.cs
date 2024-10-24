using UserManagement.Repository.Impl;

namespace UserManagement.Repository;

public interface IUnitOfWork : IDisposable
{ 
    IUserRepo UserRepo { get; }
    ITeamRepository TeamRepository { get; }
    ISportRepository SportRepository { get; }
    IConservationRepository ConservationRepository { get; }
    Task<int> SaveChangesAsync();
}