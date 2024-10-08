﻿using Entity;

namespace MatchManagement.Repository.Impl;

public class MatchRepo(RallywaveContext repositoryContext) : RepositoryBase<Match>(repositoryContext),IMatchRepo
{
    public async Task<List<Match>> GetMatches(string? filterField, string? filterValue)
    {
        var matches = new List<Match>();
        try
        {
            switch (filterField!.ToLower())
                {
                    case "teamsize":
                        matches = await FindByConditionAsync(m => m.TeamSize.Equals(filterValue), 
                            m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        break;
                    case "minlevel":
                        if (sbyte.TryParse(filterValue, out var minLevel))
                        {
                            matches = await FindByConditionAsync(m => m.MinLevel.Equals(minLevel),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "maxlevel":
                        if (sbyte.TryParse(filterValue, out var maxLevel))
                        {
                            matches = await FindByConditionAsync(m => m.MinLevel.Equals(maxLevel),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "mode":
                        if (sbyte.TryParse(filterValue, out var mode))
                        {
                            matches = await FindByConditionAsync(m => m.Mode.Equals(mode),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "minage":
                        matches = await FindByConditionAsync(m =>
                            m.MinAge != null && m.MinAge.Equals(sbyte.Parse(filterValue!)),
                            m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        break;
                    case "maxage":
                        matches = await FindByConditionAsync(m =>
                                m.MinAge != null && m.MaxAge.Equals(sbyte.Parse(filterValue!)),
                            m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        break;
                    case "gender":
                        matches = await FindByConditionAsync(m => m.Gender != null && m.Gender.Equals(filterValue),
                            m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        break;
                    case "date":
                        if (DateOnly.TryParse(filterValue, out var date))
                        {
                            matches = await FindByConditionAsync(m => m.Date.Equals(date),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "timestart":
                        if (TimeOnly.TryParse(filterValue, out var timeStart))
                        {
                            matches = await FindByConditionAsync(m => m.TimeStart.Equals(timeStart),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "timeend":
                        if (TimeOnly.TryParse(filterValue, out var timeEnd))
                        {
                            matches = await FindByConditionAsync(m => m.TimeStart.Equals(timeEnd),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
                        }
                        break;
                    case "matchtype":
                        if (sbyte.TryParse(filterValue, out var type))
                        {
                            matches = await FindByConditionAsync(m => m.MatchType.Equals(type),
                                m => m.Booking, m => m.UserMatches, m => m.Conservation, m => m.Sport!);
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