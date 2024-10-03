using Entity;


namespace MatchManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    public IMatchRepo matchRepo { get; } = new MatchRepo(context);
}