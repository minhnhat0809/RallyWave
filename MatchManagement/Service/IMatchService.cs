using MatchManagement.DTOs;
using MatchManagement.DTOs.MatchDto;

namespace MatchManagement.Service;

public interface IMatchService
{
    Task<ResponseDto> GetMatches(string? subject, int? subjectId, MatchFilterDto? matchFilterDto, string? sortField, string sortValue , int pageNumber, int pageSize);

    Task<ResponseDto> EnrollInMatch(int userId, int matchId);

    Task<ResponseDto> UnEnrollFromMatch(int userId, int matchId);

    Task<ResponseDto> GetMatchById(int id);

    Task<ResponseDto> CreateMatch(int userId, MatchCreateDto matchCreateDto);

    Task<ResponseDto> UpdateMatch(int id, MatchUpdateDto matchUpdateDto);

    Task<ResponseDto> DeleteMatch(int id);
}