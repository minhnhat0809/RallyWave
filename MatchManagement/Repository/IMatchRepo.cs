using Entity;
using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement.Repository;

public interface IMatchRepo : IRepositoryBase<Match>
{
    Task<ResponseListDto<MatchViewsDto>> GetMatches(string? subject, int? subjectId, 
        MatchFilterDto? matchFilterDto, 
        string? sortField, string sortValue,
        int pageNumber, int pageSize);
}