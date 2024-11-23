namespace MatchManagement.Repository;

public interface IUnitOfWork
{
    IMatchRepo MatchRepo { get; }
    
    ISportRepo SportRepo { get; }
    
    IUserRepo UserRepo { get; }
    
    IUserMatchRepo UserMatchRepo { get; }
    
    IUserSportRepo UserSportRepo { get; }
    
    IFriendShipRepo FriendShipRepo { get; }
    
    ITeamRepo TeamRepo { get; }
}