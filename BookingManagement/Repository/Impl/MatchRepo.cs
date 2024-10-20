using Entity;

namespace BookingManagement.Repository.Impl;

public class MatchRepo(RallyWaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext), IMatchRepo
{
    
}