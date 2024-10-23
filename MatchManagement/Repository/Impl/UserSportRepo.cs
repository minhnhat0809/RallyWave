using Entity;

namespace MatchManagement.Repository.Impl;

public class UserSportRepo(RallyWaveContext repositoryContext) : RepositoryBase<UserSport>(repositoryContext), IUserSportRepo
{
    
}