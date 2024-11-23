using Entity;

namespace MatchManagement.Repository.Impl;

public class TeamRepo(RallyWaveContext repositoryContext) : RepositoryBase<Team>(repositoryContext), ITeamRepo
{
    
}