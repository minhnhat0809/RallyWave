using Entity;

namespace BookingManagement.Repository.Impl;

public class CourtRepo(RallyWaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext),ICourtRepo
{
    public async Task<Court?> GetCourtById(int id)
    {
        try
        {
            return await GetByConditionAsync(c => c.CourtId == id, c => c);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}