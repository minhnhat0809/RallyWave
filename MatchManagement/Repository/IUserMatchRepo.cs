using Entity;

namespace MatchManagement.Repository;

public interface IUserMatchRepo : IRepositoryBase<UserMatch>
{
    Task UnEnrollment(UserMatch userMatch);
}