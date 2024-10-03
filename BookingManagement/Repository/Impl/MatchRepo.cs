using Entity;

namespace BookingManagement.Repository.Impl;

public class MatchRepo(RallywaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext), IMatchRepo
{
    
}