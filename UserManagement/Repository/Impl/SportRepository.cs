using Entity;

namespace UserManagement.Repository.Impl;

public interface ISportRepository : IRepositoryBase<Sport>
{
    Task<Sport?> GetSportById(int id);
}
public class SportRepository(RallyWaveContext repositoryContext) : RepositoryBase<Sport>(repositoryContext), ISportRepository
{
    public async Task<Sport?> GetSportById(int id)
    {
        return await repositoryContext.Sports.FindAsync(id);
    }
}