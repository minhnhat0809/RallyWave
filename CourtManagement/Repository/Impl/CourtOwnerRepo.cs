using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtOwnerRepo(RallyWaveContext repositoryContext) : RepositoryBase<CourtOwner>(repositoryContext), ICourtOwnerRepo
{
    
}