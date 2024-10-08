namespace MatchManagement.Repository;

public interface IUnitOfWork
{
    IMatchRepo MatchRepo { get; }
    
    ISportRepo SportRepo { get; }
}