using Entity;

namespace MatchManagement.Repository.Impl;

public class SportRepo(RallywaveContext repositoryContext) : RepositoryBase<Sport>(repositoryContext), ISportRepo
{
    
}