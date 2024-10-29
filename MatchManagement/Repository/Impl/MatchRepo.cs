using System.Linq.Expressions;
using Entity;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository.Impl;

public class MatchRepo(RallyWaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext),IMatchRepo
{
    public async Task<ResponseListDto<MatchViewsDto>> GetMatches(string filterField, string filterValue, int pageNumber, int pageSize)
    {
        try
        {
            Expression<Func<Match, bool>> basePredicate = m => true;
            
            switch (filterField.ToLower())
            {
                case "matchname":
                    basePredicate = m => m.MatchName.Equals(filterValue, StringComparison.InvariantCultureIgnoreCase);
                    break;
                
                case "teamsize":
                    var teamSize = sbyte.Parse(filterValue);
                    basePredicate = m => m.TeamSize == teamSize;
                    break;

                case "minlevel":
                    if (sbyte.TryParse(filterValue, out var minLevel))
                    {
                        basePredicate = m => m.MinLevel == minLevel;
                    }
                    break;

                case "maxlevel":
                    if (sbyte.TryParse(filterValue, out var maxLevel))
                    {
                        basePredicate = m => m.MaxLevel == maxLevel;
                    }
                    break;

                case "mode":
                    if (sbyte.TryParse(filterValue, out var mode))
                    {
                        basePredicate = m => m.Mode == mode;
                    }
                    break;

                case "minage":
                    var minAge = sbyte.Parse(filterValue);
                    basePredicate = m => m.MinAge != null && m.MinAge == minAge;
                    break;

                case "maxage":
                    var maxAge = sbyte.Parse(filterValue);
                    basePredicate = m => m.MaxAge != null && m.MaxAge == maxAge;
                    break;

                case "gender":
                    basePredicate = m => m.Gender != null && m.Gender == filterValue;
                    break;

                case "date":
                    if (DateOnly.TryParse(filterValue, out var date))
                    {
                        basePredicate = m => m.Date == date;
                    }
                    break;

                case "timestart":
                    if (TimeOnly.TryParse(filterValue, out var timeStart))
                    {
                        basePredicate = m => m.TimeStart == timeStart;
                    }
                    break;

                case "timeend":
                    if (TimeOnly.TryParse(filterValue, out var timeEnd))
                    {
                        basePredicate = m => m.TimeEnd == timeEnd;
                    }
                    break;

                case "matchtype":
                    if (sbyte.TryParse(filterValue, out var matchType))
                    {
                        basePredicate = m => m.MatchType == matchType;
                    }
                    break;
            }
            
            var matches = await FindByConditionWithPagingAsync(
                basePredicate,
                m => new MatchViewsDto(
                    m.MatchId, 
                    m.Sport.SportName, 
                    m.MatchName, 
                    m.CreateBy,
                    m.CreateByNavigation.UserName,
                    m.MatchType,
                    m.TeamSize,
                    m.MinLevel,
                    m.MaxLevel,
                    m.Date,
                    m.TimeStart,
                    m.TimeEnd,
                    m.Location!,
                    m.Status ?? 0
                ), pageNumber, pageSize);
            
            var total = await CountByConditionAsync(basePredicate);
            
            var responseDto = new ResponseListDto<MatchViewsDto>(matches, total);
            
            return responseDto;
            
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}