using Entity;

namespace Identity.API.Repository.Impl;

public class UnitOfWork(RallywaveContext context, IConfiguration configuration) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
    public IAuthRepository AuthRepository { get; } = new AuthRepository(configuration);
    public ICourtOwnerRepository CourtOwnerRepository { get; } = new CourtOwnerRepository(context);
}