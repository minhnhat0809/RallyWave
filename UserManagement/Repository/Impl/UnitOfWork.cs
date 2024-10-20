using Entity;

namespace UserManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
}