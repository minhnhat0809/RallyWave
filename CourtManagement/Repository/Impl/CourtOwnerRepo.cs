using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtOwnerRepo(RallywaveContext repositoryContext) : RepositoryBase<CourtOwner>(repositoryContext), ICourtOwnerRepo
{
    
}