using Entity;

namespace ChattingManagement.Repository.Impl;

public class ConservationRepo(RallywaveContext repositoryContext) : RepositoryBase<Conservation>(repositoryContext), IConservationRepo
{
    
}