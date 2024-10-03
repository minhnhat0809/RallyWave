using Entity;

namespace MatchManagement.Repository;

public interface IMatchRepo : IRepositoryBase<Match>
{
    Task<List<Match>> GetMatches(string? filterField, string? filterValue);
}