using Entity;

namespace BookingManagement.Repository;

public interface ICourtRepo : IRepositoryBase<Court>
{
    Task<Court?> GetCourtById(int id);
}