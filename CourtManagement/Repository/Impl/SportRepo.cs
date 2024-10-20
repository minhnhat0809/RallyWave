using Entity;

namespace CourtManagement.Repository.Impl;

public class SportRepo(RallyWaveContext repositoryContext) : RepositoryBase<Sport>(repositoryContext), ISportRepo
{
    
}