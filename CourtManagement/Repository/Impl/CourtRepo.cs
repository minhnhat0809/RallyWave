using CourtManagement.DTOs.CourtDto.ViewDto;
using Entity;

namespace CourtManagement.Repository.Impl;

public class CourtRepo(RallywaveContext repositoryContext) : RepositoryBase<Court>(repositoryContext), ICourtRepo
{
    public async Task<List<CourtsViewDto>> GetCourts(string? filterField, string? filterValue)
    {
        var courts = new List<CourtsViewDto>();
        try
        {
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                courts =  await FindAllAsync(c => new CourtsViewDto(c.CourtId, c.CourtName, c.Address, c.Province, c.Status, c.Sport!.SportName));
            }
            else
            {
                switch (filterValue.ToLower())
                {
                    case "sport":
                        courts =  await FindByConditionAsync(c => c.Sport!.SportId.Equals(int.Parse(filterValue)),
                            c => new CourtsViewDto(c.CourtId, c.CourtName, c.Address, c.Province, c.Status, c.Sport!.SportName));
                        break;
                    case "status":
                        if (sbyte.TryParse(filterValue, out var status))
                        {
                            courts =  await FindByConditionAsync(c => c.Status.Equals(status),
                                c => new CourtsViewDto(c.CourtId, c.CourtName, c.Address, c.Province, c.Status, c.Sport!.SportName));
                        }
                        break;
                    case "maxplayers":
                        var number = sbyte.Parse(filterValue);
                        courts =  await FindByConditionAsync(c => c.MaxPlayers.Equals(number),
                            c => new CourtsViewDto(c.CourtId, c.CourtName, c.Address, c.Province, c.Status, c.Sport!.SportName));
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
            return await GetByIdAsync(courtId, c => c);
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