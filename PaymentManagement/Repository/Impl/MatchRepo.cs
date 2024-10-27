using Entity;

namespace PaymentManagement.Repository.Impl;

public class MatchRepo(RallyWaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext), IMatchRepo
{
    
}