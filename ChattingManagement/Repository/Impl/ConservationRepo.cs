using System.Runtime.CompilerServices;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace ChattingManagement.Repository.Impl;

public interface IConservationRepo : IRepositoryBase<Conservation>
{
    Task<Conservation?> GetConservationByProperties(string properties, string propertiesValue);
    Task<List<Conservation?>> GetConservationsByProperties(string properties, string propertiesValue);
    Task<Conservation> CreateConservationAsync(Conservation conservation);
    Task<Conservation> UpdateConservationAsync(Conservation conservation);
    Task<Conservation> DeleteConservationAsync(Conservation conservation);
}
public class ConservationRepo(RallyWaveContext repositoryContext) : RepositoryBase<Conservation>(repositoryContext), IConservationRepo
{
    private readonly RallyWaveContext _repositoryContext = repositoryContext;

    public async Task<Conservation?> GetConservationByProperties(string properties, string propertiesValue)
    {
        var conservation = await repositoryContext.Conservations
            .Include(x => x.Users)
            .Include(x => x.Match)
            .Include(x => x.Team)
            .FirstOrDefaultAsync(c => EF.Property<string>(c, properties.ToLower()) == propertiesValue.ToLower());
        return conservation;
    }

    public async Task<List<Conservation?>> GetConservationsByProperties(string? properties, string propertiesValue)
    {
        // If no properties are provided, return all conservations with included navigation properties
        if (string.IsNullOrEmpty(properties))
        {
            var listConservation = await _repositoryContext.Conservations
                .Include(x => x.Users)
                .Include(x => x.Match)
                .Include(x => x.Team)
                .ToListAsync();
            return listConservation!;
        }

        // Using EF.Property to dynamically filter based on the property name
        var query = _repositoryContext.Conservations
            .Include(x => x.Users)
            .Include(x => x.Match)
            .Include(x => x.Team)
            .AsQueryable();

        try
        {
            // Applying the filter
            query = query.Where(c => EF.Property<string>(c, properties.ToLower()) == propertiesValue.ToLower());
        }
        catch (ArgumentException ex)
        {
            throw new Exception($"The property '{properties}' does not exist on the Conservation entity.", ex);
        }

        // Return the filtered list
        return await query.ToListAsync();
    }


    public async Task<Conservation> CreateConservationAsync(Conservation conservation)
    {
        _repositoryContext.Conservations.Add(conservation);
        await _repositoryContext.SaveChangesAsync();
        return conservation;
    }

    public async Task<Conservation> UpdateConservationAsync(Conservation conservation)
    {
        _repositoryContext.Conservations.Update(conservation);
        await _repositoryContext.SaveChangesAsync();
        return conservation;
    }

    public async Task<Conservation> DeleteConservationAsync(Conservation conservation)
    {
        _repositoryContext.Conservations.Remove(conservation);
        await _repositoryContext.SaveChangesAsync();
        return conservation;
    }
}