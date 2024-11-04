using UserManagement.Repository.Impl;

namespace UserManagement.Repository;

public interface IUnitOfWork : IDisposable
{ 
    IUserRepo UserRepo { get; }
    ITeamRepository TeamRepository { get; }
    ISportRepository SportRepository { get; }
    IConservationRepository ConservationRepository { get; }
    IUserTeamRepository UserTeamRepository { get; }
    IFriendRepository FriendRepository { get; }
    Task<int> SaveChangesAsync();
    ICourtOwnerRepository CourtOwnerRepository { get; }
}