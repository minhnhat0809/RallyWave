using Entity;

namespace MatchManagement.Repository.Impl;

public class UserMatchRepo(RallywaveContext repositoryContext) : RepositoryBase<UserMatch>(repositoryContext), IUserMatchRepo
{
    
}