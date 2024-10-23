using Entity;
using UserManagement.Repository;

namespace Identity.API.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
}