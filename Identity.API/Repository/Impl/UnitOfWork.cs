using Entity;

namespace Identity.API.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
    public IAuthRepository AuthRepository { get; } = new AuthRepository(configuration);
    public ICourtOwnerRepository CourtOwnerRepository { get; } = new CourtOwnerRepository(context);
}