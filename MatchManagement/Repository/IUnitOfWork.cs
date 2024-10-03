namespace MatchManagement.Repository;

public interface IUnitOfWork
{
    IMatchRepo matchRepo { get; }
}