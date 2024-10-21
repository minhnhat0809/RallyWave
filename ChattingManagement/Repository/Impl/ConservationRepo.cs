using Entity;

namespace ChattingManagement.Repository.Impl;

public class ConservationRepo(RallyWaveContext repositoryContext) : RepositoryBase<Conservation>(repositoryContext), IConservationRepo
{
    
}