using Entity;

namespace Identity.API.Repository.Impl;

public class UnitOfWork(RallyWaveContext context, IConfiguration configuration) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
    public IAuthRepository AuthRepository { get; } = new AuthRepository(configuration);
    public ICourtOwnerRepository CourtOwnerRepository { get; } = new CourtOwnerRepository(context);
    public IFirebaseStorageRepository FirebaseStorageRepository { get; } = new FirebaseStorageRepository(configuration);

}