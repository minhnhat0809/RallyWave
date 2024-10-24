using Entity;

namespace UserManagement.Repository.Impl;

public interface IConservationRepository : IRepositoryBase<Conservation>
{
    Task<Conservation> AddConservationAsync(Conservation conservation);
}
public class ConservationRepository(RallyWaveContext repositoryContext) : RepositoryBase<Conservation>(repositoryContext), IConservationRepository
{
    public async Task<Conservation> AddConservationAsync(Conservation conservation)
    {
        var isCreated = await CreateAsync(conservation);
        if (isCreated)
        {
            return conservation;
        }
        throw new Exception("Failed to create team.");
    }
}