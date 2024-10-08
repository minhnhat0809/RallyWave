using Entity;


namespace MatchManagement.Repository.Impl;

public class UnitOfWork(RallywaveContext context) : IUnitOfWork
{
    public IMatchRepo MatchRepo { get; } = new MatchRepo(context);

    public ISportRepo SportRepo { get; } = new SportRepo(context);
}