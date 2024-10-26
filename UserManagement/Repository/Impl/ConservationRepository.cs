using Entity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Repository.Impl;

public interface IConservationRepository : IRepositoryBase<Conservation>
{
    Task<Conservation> AddConservationAsync(Conservation conservation);
    Task<Conservation?> GetConservationByIdAsync(int id);
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

    public async Task<Conservation?> GetConservationByIdAsync(int id)
    {
        return await repositoryContext.Conservations
            .Include(x=>x.Users)
            .FirstOrDefaultAsync(x=>x.ConservationId == id);
    }
}