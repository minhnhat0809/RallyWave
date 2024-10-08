using Entity;

namespace CourtManagement.Repository.Impl;

public class SportRepo(RallywaveContext repositoryContext) : RepositoryBase<Sport>(repositoryContext), ISportRepo
{
    
}