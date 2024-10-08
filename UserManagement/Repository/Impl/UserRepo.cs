using Entity;

namespace UserManagement.Repository.Impl;

public class UserRepo(RallywaveContext repositoryContext) : RepositoryBase<User>(repositoryContext),IUserRepo
{
    
}