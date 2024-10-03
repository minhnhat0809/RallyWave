using CourtManagement.Enum;
using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtRepo(RallywaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext), ICourtRepo
{
    public async Task<List<Court>> GetCourts(string? filterField, string? filterValue)
    {
        var courts = new List<Court>();
        try
        {
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                courts =  await FindAllAsync( c => c.Bookings, c => c.CourtOwner!, c => c.Sport, c => c.Slots);
            }
            else
            {
                switch (filterValue.ToLower())
                {
                    case "sport":
                        courts =  await FindByConditionAsync(c => c.Sport!.SportId.Equals(int.Parse(filterValue)),
                            c => c.Bookings, c => c.CourtOwner!, c => c.Sport, c => c.Slots);
                        break;
                    
                    case "status":
                        if (System.Enum.TryParse<CourtStatus>(filterValue, true, out var status))
                        {
                            courts =  await FindByConditionAsync(c => c.Status.Equals(status),
                                c => c.Bookings, c => c.CourtOwner!, c => c.Sport, c => c.Slots);
                        }
                        break;
                    
                    case "maxplayers":
                        var number = sbyte.Parse(filterValue);
                        courts =  await FindByConditionAsync(c => c.MaxPlayers.Equals(number),
                            c => c.Bookings, c => c.CourtOwner!, c => c.Sport, c => c.Slots);
                        break;
                    
                }
            }
            
            return courts;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Court?> GetCourtById(int courtId)
    {
        try
        {
            return await GetByIdAsync(courtId,  c => c.Bookings, c => c.CourtOwner!, c => c.Sport, c => c.Slots);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task CreateCourt(Court court)
    {
        try
        {
            await CreateAsync(court);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task UpdateCourt(Court court)
    {
        try
        {
            await UpdateAsync(court);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteCourt(Court court)
    {
        try
        {
            await DeleteAsync(court);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}