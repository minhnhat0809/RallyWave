using Entity;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository;

public interface IMatchRepo : IRepositoryBase<Match>
{
    Task<List<MatchViewsDto>> GetMatches(string filterField, string filterValue, int pageNumber, int pageSize);
}