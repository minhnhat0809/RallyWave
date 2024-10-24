using Entity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Repository.Impl;

public interface ITeamRepository : IRepositoryBase<Team>
{
    Task<List<Team>> GetTeams(string? filterField, string? filterValue);

    Task<Team?> GetTeamById(int id);

    Task<Team> CreateTeam(Team team);

    Task<Team> UpdateTeam(Team team);

    Task<Team> DeleteTeam(Team team);
}

public class TeamRepository(RallyWaveContext repositoryContext) : RepositoryBase<Team>(repositoryContext), ITeamRepository
{
        // Get all teams with optional filtering
        public async Task<List<Team>> GetTeams(string? filterField, string? filterValue)
        {
            // If no filter is provided, return all teams
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                return await repositoryContext.Teams
                    .Include(team => team.Sport)
                    .Include(team => team.UserTeams)
                    .Include(team => team.Conservation)
                    .Include(team => team.CreateByNavigation)
                    .ToListAsync();
                    
            }

            // Handle dynamic filtering based on the provided filterField and filterValue
            if (filterField.Equals("TeamName", StringComparison.OrdinalIgnoreCase))
            {
                return await FindByConditionAsync(t => t.TeamName.Contains(filterValue), t => t);
            }
            else if (filterField.Equals("Status", StringComparison.OrdinalIgnoreCase))
            {
                if (sbyte.TryParse(filterValue, out sbyte status))
                {
                    return await FindByConditionAsync(t => t.Status == status, t => t);
                }
                else
                {
                    throw new ArgumentException("Invalid status value");
                }
            }
            else
            {
                throw new ArgumentException("Invalid filter field");
            }
        }

        // Get a team by ID
        public async Task<Team?> GetTeamById(int teamId)
        {
            var team = await repositoryContext.Teams
                .Include(x=>x.CreateByNavigation)
                .Include(x=>x.UserTeams)
                .Include(x=>x.Conservation)
                .Include(x=>x.Sport)
                .FirstOrDefaultAsync(x=>x.TeamId == teamId);
            return team;
        }

        // Create a new team
        public async Task<Team> CreateTeam(Team team)
        {
            var isCreated = await CreateAsync(team);
            if (isCreated)
            {
                return team;
            }
            throw new Exception("Failed to create team.");
        }

        // Update an existing team
        public async Task<Team> UpdateTeam(Team team)
        {
            var isUpdated = await UpdateAsync(team);
            if (isUpdated)
            {
                return team;
            }
            throw new Exception("Failed to update team.");
        }

        // Delete a team
        public async Task<Team> DeleteTeam(Team team)
        {
            var isDeleted = await DeleteAsync(team);
            if (isDeleted)
            {
                return team;
            }
            throw new Exception("Failed to delete team.");
        }
    }