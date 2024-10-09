using Entity;

namespace UserManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    public IUserRepo UserRepo { get; } = new UserRepo(context);
}