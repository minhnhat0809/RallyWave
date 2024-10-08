using Entity;

namespace CourtManagement.Repository;

public interface ICourtImageRepo : IRepositoryBase<CourtImage>
{
    Task DeleteCourtImage(CourtImage courtImage);
}