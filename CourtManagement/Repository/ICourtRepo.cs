using Entity;

namespace CourtManagement.Repository;

public interface ICourtRepo : IRepositoryBase<Court>
{
    Task<List<Court>> GetCourts(string? filterField, string? filterValue);

    Task<Court?> GetCourtById(int courtId);

    Task CreateCourt(Court court);

    Task UpdateCourt(Court court);

    Task DeleteCourt(Court court);
}