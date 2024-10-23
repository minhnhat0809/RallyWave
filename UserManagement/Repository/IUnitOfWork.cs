using UserManagement.Repository.Impl;

namespace UserManagement.Repository;

public interface IUnitOfWork
{ 
    IUserRepo UserRepo { get; }
    ITeamRepository TeamRepository { get; }
}