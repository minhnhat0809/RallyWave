using Entity;
using Microsoft.EntityFrameworkCore;

namespace ChattingManagement.Repository.Impl;

public interface ITeamRepository : IRepositoryBase<Team>
{
    Task<Team?> GetTeamById(int id);
}
public class TeamRepository(RallyWaveContext repositoryContext) : RepositoryBase<Team>(repositoryContext), ITeamRepository
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task<Team?> GetTeamById(int id)
    {
        return await _repositoryContext.Teams
            .Include(x => x.Conservation)
            .FirstOrDefaultAsync(x => x.TeamId == id);
    }
}