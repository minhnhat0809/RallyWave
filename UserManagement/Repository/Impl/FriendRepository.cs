using Entity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Repository.Impl;

public interface IFriendRepository : IRepositoryBase<Friendship>
{
    Task<List<Friendship>> GetFriendShipByProperties(int userId, string filter, string value);
    // status = 0 is created send request (not accepted yet)
    // status = 1 is being friend (accepted)
    
    // get all friend by user id [status 1]
    // get all friend request by user id [status = 0, receiver is userId]
    // get all friend sending by user id [status = 0, sender is userId]
    Task<Friendship> CreateFriendShip(Friendship friendship);
    Task<Friendship> AcceptedFriendShip(Friendship friendship);
    // Accept Friend Request
    Task<Friendship> DeniedFriendShip(Friendship friendship);
    // Deny Friend Request

    Task<Friendship?> GetFriendShip(int userId, int senderId);
}
public class FriendRepository(RallyWaveContext repositoryContext) : RepositoryBase<Friendship>(repositoryContext), IFriendRepository
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task<List<Friendship>> GetFriendShipByProperties(int userId, string filter, string value)
    {
        IQueryable<Friendship> query = _repositoryContext.Friendships
            .Include(f => f.Sender)
            .Include(f => f.Receiver);
        switch (filter.ToLower())
        {
            case "all-friends" :
                // Get all accepted friends
                query = query.Where(f => f.Status == 1 && (f.SenderId == userId || f.ReceiverId == userId));
                break;

            case "received-requests":
                // Get all friend requests received by the user (status = 0, receiver is userId)
                query = query.Where(f => f.Status == 0 && f.ReceiverId == userId);
                break;

            case "sent-requests":
                // Get all friend requests sent by the user (status = 0, sender is userId)
                query = query.Where(f => f.Status == 0 && f.SenderId == userId);
                break;

            default:
                throw new ArgumentException("Invalid filter type specified.");
        }

        return await query.ToListAsync();
    }

    public async Task<Friendship> CreateFriendShip(Friendship friendship)
    {
        _repositoryContext.Friendships.Add(friendship);
        await _repositoryContext.SaveChangesAsync();
        return friendship;
    }

    public async Task<Friendship> AcceptedFriendShip(Friendship friendship)
    {
        _repositoryContext.Update(friendship);
        await _repositoryContext.SaveChangesAsync();
        return friendship;
    }

    public async Task<Friendship> DeniedFriendShip(Friendship friendship)
    {
        _repositoryContext.Remove(friendship);
        await _repositoryContext.SaveChangesAsync();
        return friendship;
    }

    public async Task<Friendship?> GetFriendShip(int senderId, int receiverId)
    {
        // make sure that exist their friend
        var friendShip = await 
            repositoryContext.Friendships
                .FirstOrDefaultAsync(x => x.ReceiverId == senderId && x.SenderId == receiverId);
        if (friendShip == null)
        {
            return await repositoryContext.Friendships
                .FirstOrDefaultAsync(x => x.ReceiverId == receiverId && x.SenderId == senderId);
        }
        return friendShip;
    }
}