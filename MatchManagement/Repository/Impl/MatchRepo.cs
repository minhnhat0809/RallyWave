using System.Linq.Expressions;
using Entity;
using LinqKit;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository.Impl;

public class MatchRepo(RallyWaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext),IMatchRepo
{
    public async Task<ResponseListDto<MatchViewsDto>> GetMatches(
    string? subject, int? subjectId, 
    MatchFilterDto? matchFilterDto, 
    string? sortField, string sortValue,
    int pageNumber, int pageSize)
{
    try
    {
        // Start with a base predicate that is always true
            var basePredicate = PredicateBuilder.New<Match>(true);

        // Check for subject and subjectId
        if (!string.IsNullOrWhiteSpace(subject) && subjectId.HasValue)
        {
            basePredicate = subject.ToLower() switch
            {
                "user" => basePredicate.And(m => m.CreateBy == subjectId.Value),
                _ => throw new ArgumentException($"Unknown subject '{subject}'")
            };
        }

        // Add filter conditions from matchFilterDto
        if (matchFilterDto != null)
        {
            if (!string.IsNullOrWhiteSpace(matchFilterDto.MatchName))
            {
                basePredicate = basePredicate.And(m => m.MatchName.Contains(matchFilterDto.MatchName));
            }

            // Using null-conditional operator and short-circuit evaluation
            if (matchFilterDto.TeamSize.HasValue)
            {
                basePredicate = basePredicate.And(m => m.TeamSize == matchFilterDto.TeamSize);
            }

            if (matchFilterDto.MinLevel.HasValue)
            {
                basePredicate = basePredicate.And(m => m.MinLevel == matchFilterDto.MinLevel);
            }

            if (matchFilterDto.MaxLevel.HasValue)
            {
                basePredicate = basePredicate.And(m => m.MaxLevel == matchFilterDto.MaxLevel);
            }

            if (matchFilterDto.Mode.HasValue)
            {
                basePredicate = basePredicate.And(m => m.Mode == matchFilterDto.Mode);
            }

            if (matchFilterDto.MinAge.HasValue)
            {
                basePredicate = basePredicate.And(m => m.MinAge == matchFilterDto.MinAge);
            }

            if (matchFilterDto.MaxAge.HasValue)
            {
                basePredicate = basePredicate.And(m => m.MaxAge == matchFilterDto.MaxAge);
            }

            if (!string.IsNullOrWhiteSpace(matchFilterDto.Gender))
            {
                basePredicate = basePredicate.And(m => m.Gender == matchFilterDto.Gender);
            }

            // Handle Date Filters
            if (matchFilterDto.Date.HasValue)
            {
                basePredicate = basePredicate.And(m => m.Date == matchFilterDto.Date.Value);
            }
            else
            {
                if (matchFilterDto.DateFrom.HasValue)
                {
                    basePredicate = basePredicate.And(m => m.Date >= matchFilterDto.DateFrom.Value);
                }

                if (matchFilterDto.DateTo.HasValue)
                {
                    basePredicate = basePredicate.And(m => m.Date <= matchFilterDto.DateTo.Value);
                }
            }

            if (matchFilterDto.TimeStart.HasValue)
            {
                basePredicate = basePredicate.And(m => m.TimeStart >= matchFilterDto.TimeStart.Value);
            }

            if (matchFilterDto.TimeEnd.HasValue)
            {
                basePredicate = basePredicate.And(m => m.TimeEnd <= matchFilterDto.TimeEnd.Value);
            }

            if (matchFilterDto.MatchType.HasValue)
            {
                basePredicate = basePredicate.And(m => m.MatchType == matchFilterDto.MatchType);
            }

            if (matchFilterDto.Status.HasValue)
            {
                basePredicate = basePredicate.And(m => m.Status == matchFilterDto.Status.Value);
            }
        }

        // Default sorting field if not provided
        sortField ??= "timestart";
        
        // Determine sorting order
        var isAscending = sortValue.Equals("asc", StringComparison.OrdinalIgnoreCase);
        
        // Using the existing FindByConditionWithSortingAndPagingAsync
        var matches = await FindByConditionWithSortingAndPagingAsync(
            basePredicate,
            m => new MatchViewsDto(
                m.MatchId,
                m.Sport.SportName,
                m.MatchName,
                m.CreateBy,
                m.CreateByNavigation.UserName,
                m.MatchType,
                m.UserMatches.Count,
                m.TeamSize,
                m.MinLevel,
                m.MaxLevel,
                m.Date,
                m.TimeStart,
                m.TimeEnd,
                m.Location!,
                m.Status ?? 0
            ),
            pageNumber, pageSize,
            m => m.Date, // primary sorting field
            sortField switch
            {
                "timestart" => m => m.TimeStart,
                "timeend" => m => m.TimeEnd,
                "status" => m => m.Status!,
                "teamsize" => m => m.TeamSize,
                "minlevel" => m => m.MinLevel ?? 0,
                "maxlevel" => m => m.MaxLevel ?? 5,
                _ => throw new ArgumentException($"Unknown sorting column '{sortField}'")
            },
            isAscending,
            isAscending
        );

        var total = await CountByConditionAsync(basePredicate);
        var responseDto = new ResponseListDto<MatchViewsDto>(matches, total);
        return responseDto;
    }
    catch (Exception e)
    {
        // Consider more specific error handling
        throw new Exception(e.Message);
    }
}


}