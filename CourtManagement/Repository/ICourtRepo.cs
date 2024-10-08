using CourtManagement.DTOs.CourtDto.ViewDto;
using Entity;

namespace CourtManagement.Repository;

public interface ICourtRepo : IRepositoryBase<Court>
{
    Task<List<CourtsViewDto>> GetCourts(string? filterField, string? filterValue);

    Task<Court?> GetCourtById(int courtId);

    Task CreateCourt(Court court);

    Task UpdateCourt(Court court);

    Task DeleteCourt(Court court);
}