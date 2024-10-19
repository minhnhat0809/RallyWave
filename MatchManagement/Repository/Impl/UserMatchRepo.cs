using Entity;

namespace MatchManagement.Repository.Impl;

public class UserMatchRepo(RallywaveContext repositoryContext) : RepositoryBase<UserMatch>(repositoryContext), IUserMatchRepo
{
    public async Task UnEnrollment(UserMatch userMatch)
    {
        try
        {
            //delete in database
            repositoryContext.UserMatches.Remove(userMatch);
            
            await repositoryContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}