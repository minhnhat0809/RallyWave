using Entity;

namespace MatchManagement.Repository.Impl;

public class UserRepo(RallywaveContext repositoryContext) : RepositoryBase<User>(repositoryContext), IUserRepo
{
    
}