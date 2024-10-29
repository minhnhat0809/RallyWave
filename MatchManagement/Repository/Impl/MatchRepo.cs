using System.Linq.Expressions;
using Entity;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository.Impl;

public class MatchRepo(RallyWaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext),IMatchRepo
{
    public async Task<ResponseListDto<MatchViewsDto>> GetMatches(MatchFilterDto matchFilterDto, int pageNumber, int pageSize)
{
    try
    {
        Expression<Func<Match, bool>> basePredicate = m => true;
        var parameter = basePredicate.Parameters[0]; // Extract the parameter to reuse in conditions

        // Filter by MatchName if provided
        if (!string.IsNullOrWhiteSpace(matchFilterDto.MatchName))
        {
            // Use Contains for partial matching
            var matchNameCondition = Expression.Call(
                Expression.Property(parameter, nameof(Match.MatchName)),
                typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                Expression.Constant(matchFilterDto.MatchName)
            );

            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, matchNameCondition),
                basePredicate.Parameters
            );
        }

        // Filter by TeamSize if provided
        if (matchFilterDto.TeamSize.HasValue)
        {
            var teamSizeProperty = Expression.Property(parameter, nameof(Match.TeamSize));
            var teamSizeCondition = Expression.AndAlso(
                Expression.NotEqual(teamSizeProperty, Expression.Constant(null, typeof(sbyte?))),
                Expression.Equal(teamSizeProperty, Expression.Constant((sbyte?)matchFilterDto.TeamSize.Value, typeof(sbyte?)))
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, teamSizeCondition),
                basePredicate.Parameters
            );
        }

        // Filter by MinLevel if provided
        if (matchFilterDto.MinLevel.HasValue)
        {
            var minLevelProperty = Expression.Property(parameter, nameof(Match.MinLevel));
            var minLevelCondition = Expression.AndAlso(
                Expression.NotEqual(minLevelProperty, Expression.Constant(null, typeof(sbyte?))),
                Expression.Equal(minLevelProperty, Expression.Constant((sbyte?)matchFilterDto.MinLevel.Value, typeof(sbyte?)))
            );

            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, minLevelCondition),
                basePredicate.Parameters
            );
        }


        // Filter by MaxLevel if provided
        if (matchFilterDto.MaxLevel.HasValue)
        {
            var maxLevelProperty = Expression.Property(parameter, nameof(Match.MaxLevel));
            var maxLevelCondition = Expression.AndAlso(
                Expression.NotEqual(maxLevelProperty, Expression.Constant(null, typeof(sbyte?))),
                Expression.Equal(maxLevelProperty, Expression.Constant((sbyte?)matchFilterDto.MaxLevel.Value, typeof(sbyte?)))
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, maxLevelCondition),
                basePredicate.Parameters
            );
        }

        // Filter by Mode if provided
        if (matchFilterDto.Mode.HasValue)
        {
            var modeCondition = Expression.Equal(
                Expression.Property(parameter, nameof(Match.Mode)),
                Expression.Constant(matchFilterDto.Mode.Value)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, modeCondition),
                basePredicate.Parameters
            );
        }

        // Filter by MinAge if provided
        if (matchFilterDto.MinAge.HasValue)
        {
            var minAgeProperty = Expression.Property(parameter, nameof(Match.MinAge));
            var minAgeCondition = Expression.AndAlso(
                Expression.NotEqual(minAgeProperty, Expression.Constant(null, typeof(sbyte?))),
                Expression.Equal(minAgeProperty, Expression.Constant((sbyte?)matchFilterDto.MinAge.Value, typeof(sbyte?)))
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, minAgeCondition),
                basePredicate.Parameters
            );
        }

        // Filter by MaxAge if provided
        if (matchFilterDto.MaxAge.HasValue)
        {
            var maxAgeProperty = Expression.Property(parameter, nameof(Match.MaxAge));
            var maxAgeCondition = Expression.AndAlso(
                Expression.NotEqual(maxAgeProperty, Expression.Constant(null, typeof(sbyte?))),
                Expression.Equal(maxAgeProperty, Expression.Constant((sbyte?)matchFilterDto.MaxAge.Value, typeof(sbyte?)))
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, maxAgeCondition),
                basePredicate.Parameters
            );
        }

        // Filter by Gender if provided
        if (!string.IsNullOrWhiteSpace(matchFilterDto.Gender))
        {
            var genderCondition = Expression.Equal(
                Expression.Property(parameter, nameof(Match.Gender)),
                Expression.Constant(matchFilterDto.Gender)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, genderCondition),
                basePredicate.Parameters
            );
        }

        // Filter by Date, DateFrom, and DateTo if provided
        if (matchFilterDto.Date.HasValue)
        {
            var dateCondition = Expression.Equal(
                Expression.Property(parameter, nameof(Match.Date)),
                Expression.Constant(matchFilterDto.Date.Value)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, dateCondition),
                basePredicate.Parameters
            );
        }
        else
        {
            if (matchFilterDto.DateFrom.HasValue)
            {
                var dateFromCondition = Expression.GreaterThanOrEqual(
                    Expression.Property(parameter, nameof(Match.Date)),
                    Expression.Constant(matchFilterDto.DateFrom.Value)
                );
                basePredicate = Expression.Lambda<Func<Match, bool>>(
                    Expression.AndAlso(basePredicate.Body, dateFromCondition),
                    basePredicate.Parameters
                );
            }
            
            if (matchFilterDto.DateTo.HasValue)
            {
                var dateToCondition = Expression.LessThanOrEqual(
                    Expression.Property(parameter, nameof(Match.Date)),
                    Expression.Constant(matchFilterDto.DateTo.Value)
                );
                basePredicate = Expression.Lambda<Func<Match, bool>>(
                    Expression.AndAlso(basePredicate.Body, dateToCondition),
                    basePredicate.Parameters
                );
            }
        }

        // Filter by TimeStart if provided
        if (matchFilterDto.TimeStart.HasValue)
        {
            var timeStartCondition = Expression.GreaterThanOrEqual(
                Expression.Property(parameter, nameof(Match.TimeStart)),
                Expression.Constant(matchFilterDto.TimeStart.Value)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, timeStartCondition),
                basePredicate.Parameters
            );
        }

        // Filter by TimeEnd if provided
        if (matchFilterDto.TimeEnd.HasValue)
        {
            var timeEndCondition = Expression.LessThanOrEqual(
                Expression.Property(parameter, nameof(Match.TimeEnd)),
                Expression.Constant(matchFilterDto.TimeEnd.Value)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, timeEndCondition),
                basePredicate.Parameters
            );
        }

        // Filter by MatchType if provided
        if (matchFilterDto.MatchType.HasValue)
        {
            var matchTypeCondition = Expression.Equal(
                Expression.Property(parameter, nameof(Match.MatchType)),
                Expression.Constant(matchFilterDto.MatchType.Value)
            );
            basePredicate = Expression.Lambda<Func<Match, bool>>(
                Expression.AndAlso(basePredicate.Body, matchTypeCondition),
                basePredicate.Parameters
            );
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
            pageNumber, pageSize
        );

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