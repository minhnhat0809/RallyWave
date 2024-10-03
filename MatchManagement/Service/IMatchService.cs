using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;

namespace MatchManagement.Service;

public interface IMatchService
{
    Task<ResponseDto> GetMatches(string? filterField, string? filterValue, string? sortField, string sortValue , int pageNumber, int pageSize);

    Task<ResponseDto> GetMatchById(int id);

    Task<ResponseDto> CreateMatch(MatchCreateDto matchCreateDto);

    Task<ResponseDto> UpdateMatch(int id, MatchUpdateDto matchUpdateDto);

    Task<ResponseDto> DeleteMatch(int id);
}