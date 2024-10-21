using Entity;

namespace MatchManagement.Repository.Impl;

public class SportRepo(RallyWaveContext repositoryContext) : RepositoryBase<Sport>(repositoryContext), ISportRepo
{
    
}