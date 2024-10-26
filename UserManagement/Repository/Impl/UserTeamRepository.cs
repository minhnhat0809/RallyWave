using Entity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Repository.Impl;

public interface IUserTeamRepository : IRepositoryBase<UserTeam>
{
    Task<UserTeam?> GetUserTeamByTeamIdAndUserId(int teamId, int userId);
    Task<UserTeam?> DeleteUserTeam(UserTeam userTeam); 
}
public class UserTeamRepository(RallyWaveContext repositoryContext) : RepositoryBase<UserTeam>(repositoryContext), IUserTeamRepository
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task<UserTeam?> GetUserTeamByTeamIdAndUserId(int teamId, int userId)
    {
        var userInTeam = await _repositoryContext.UserTeams.FirstOrDefaultAsync(x=>x.TeamId == teamId && x.UserId == userId);
        return userInTeam;
    }

    public async Task<UserTeam?> DeleteUserTeam(UserTeam userTeam)
    {
        _repositoryContext.UserTeams.Remove(userTeam);
        await _repositoryContext.SaveChangesAsync();
        return userTeam;
    }
}