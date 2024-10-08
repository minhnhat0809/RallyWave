using Entity;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository.Impl;

public class MatchRepo(RallywaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext),IMatchRepo
{
    public async Task<List<MatchViewsDto>> GetMatches(string? filterField, string? filterValue)
    {
        var matches = new List<MatchViewsDto>();
        try
        {
            switch (filterField!.ToLower())
                {
                    case "teamsize":
                        matches = await FindByConditionAsync(m => m.TeamSize == sbyte.Parse(filterValue!), m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        break;
                    case "minlevel":
                        if (sbyte.TryParse(filterValue, out var minLevel))
                        {
                            matches = await FindByConditionAsync(m => m.MinLevel == minLevel,m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "maxlevel":
                        if (sbyte.TryParse(filterValue, out var maxLevel))
                        {
                            matches = await FindByConditionAsync(m => m.MinLevel == maxLevel,m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "mode":
                        if (sbyte.TryParse(filterValue, out var mode))
                        {
                            matches = await FindByConditionAsync(m => m.Mode == mode,m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "minage":
                        matches = await FindByConditionAsync(m =>
                            m.MinAge != null && m.MinAge.Equals(sbyte.Parse(filterValue!)),m => 
                            new MatchViewsDto(
                                m.MatchId, 
                                m.Sport!.SportName, 
                                m.MatchName, 
                                m.CreateByNavigation!.UserName,
                                m.MatchType,
                                m.TeamSize,
                                m.MinLevel,
                                m.MaxLevel,
                                m.Date,
                                m.TimeStart,
                                m.TimeEnd,
                                m.Location!,
                                m.Status ?? 0
                            ));
                        break;
                    case "maxage":
                        matches = await FindByConditionAsync(m =>
                                m.MinAge != null && m.MaxAge.Equals(sbyte.Parse(filterValue!)),m => 
                            new MatchViewsDto(
                                m.MatchId, 
                                m.Sport!.SportName, 
                                m.MatchName, 
                                m.CreateByNavigation!.UserName,
                                m.MatchType,
                                m.TeamSize,
                                m.MinLevel,
                                m.MaxLevel,
                                m.Date,
                                m.TimeStart,
                                m.TimeEnd,
                                m.Location!,
                                m.Status ?? 0
                            ));
                        break;
                    case "gender":
                        matches = await FindByConditionAsync(m => m.Gender != null && m.Gender.Equals(filterValue),m => 
                            new MatchViewsDto(
                                m.MatchId, 
                                m.Sport!.SportName, 
                                m.MatchName, 
                                m.CreateByNavigation!.UserName,
                                m.MatchType,
                                m.TeamSize,
                                m.MinLevel,
                                m.MaxLevel,
                                m.Date,
                                m.TimeStart,
                                m.TimeEnd,
                                m.Location!,
                                m.Status ?? 0
                            ));
                        break;
                    case "date":
                        if (DateOnly.TryParse(filterValue, out var date))
                        {
                            matches = await FindByConditionAsync(m => m.Date.Equals(date),m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "timestart":
                        if (TimeOnly.TryParse(filterValue, out var timeStart))
                        {
                            matches = await FindByConditionAsync(m => m.TimeStart.Equals(timeStart),m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "timeend":
                        if (TimeOnly.TryParse(filterValue, out var timeEnd))
                        {
                            matches = await FindByConditionAsync(m => m.TimeStart.Equals(timeEnd),m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                    case "matchtype":
                        if (sbyte.TryParse(filterValue, out var type))
                        {
                            matches = await FindByConditionAsync(m => m.MatchType.Equals(type),m => 
                                new MatchViewsDto(
                                    m.MatchId, 
                                    m.Sport!.SportName, 
                                    m.MatchName, 
                                    m.CreateByNavigation!.UserName,
                                    m.MatchType,
                                    m.TeamSize,
                                    m.MinLevel,
                                    m.MaxLevel,
                                    m.Date,
                                    m.TimeStart,
                                    m.TimeEnd,
                                    m.Location!,
                                    m.Status ?? 0
                                ));
                        }
                        break;
                }
            
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return matches;
    }
}