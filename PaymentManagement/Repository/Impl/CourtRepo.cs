using Entity;

namespace PaymentManagement.Repository.Impl;

public class CourtRepo(RallyWaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext), ICourtRepo
{
    
}