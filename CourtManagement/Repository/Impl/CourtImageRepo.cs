using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtImageRepo(RallyWaveContext repositoryContext) : RepositoryBase<CourtImage>(repositoryContext), ICourtImageRepo
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task DeleteCourtImage(CourtImage courtImage)
    {
        try
        {
            _repositoryContext.CourtImages.Remove(courtImage);
            await _repositoryContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}