using Entity;

namespace BookingManagement.Repository.Impl;

public class CourtRepo(RallywaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext),ICourtRepo
{
    public async Task<Court?> GetCourtById(int id)
    {
        try
        {
            return await GetByIdAsync(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}