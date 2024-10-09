using Entity;
using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.Repository;

public interface IUserRepo : IRepositoryBase<User>
{
    Task<List<UserViewDto>> GetUsers(string? filterField, string? filterValue);

    Task<UserViewDto?> GetUserById(int userId);

    Task<UserViewDto> CreateUser(User user);

    Task<UserViewDto> UpdateUser(User user);

    Task<UserViewDto> DeleteUser(User user);
}