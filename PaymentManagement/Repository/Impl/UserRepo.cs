using Entity;

namespace PaymentManagement.Repository.Impl;

public class UserRepo(RallyWaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    
}