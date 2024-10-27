using Entity;

namespace PaymentManagement.Repository.Impl;

public class CourtOwnerRepo(RallyWaveContext repositoryContext) : RepositoryBase<CourtOwner>(repositoryContext), ICourtOwnerRepo
{
    
}