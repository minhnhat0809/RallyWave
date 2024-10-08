using CourtManagement.DTOs;
using CourtManagement.DTOs.CourtDto;

namespace CourtManagement.Service;

public interface ICourtService
{
    Task<ResponseDto> GetCourts(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber, int pageSize);

    Task<ResponseDto> GetCourtById(int id);

    Task<ResponseDto> CreateCourt(CourtCreateDto courtCreateDto);

    Task<ResponseDto> UpdateCourt(int id, CourtUpdateDto courtUpdateDto);

    Task<ResponseDto> DeleteCourt(int id);

    Task<ResponseDto> DeleteCourtImages(int imageId);
}