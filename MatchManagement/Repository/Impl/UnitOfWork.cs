using Entity;


namespace MatchManagement.Repository.Impl;

public class UnitOfWork(RallyWaveContext context) : IUnitOfWork
{
    public IMatchRepo MatchRepo { get; } = new MatchRepo(context);

    public ISportRepo SportRepo { get; } = new SportRepo(context);

    public IUserRepo UserRepo { get; } = new UserRepo(context);

    public IUserMatchRepo UserMatchRepo { get; } = new UserMatchRepo(context);
    
    public IUserSportRepo UserSportRepo { get; } = new UserSportRepo(context);

    public IFriendShipRepo FriendShipRepo { get; } = new FriendShipRepo(context);
}