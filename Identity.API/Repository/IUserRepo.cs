using Entity;
using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.Repository;

public interface IUserRepo : IRepositoryBase<User>
{
    Task<List<UserViewDto>> GetUsers(string? filterField, string? filterValue);

    Task<UserViewDto?> GetUserById(int userId);

    Task<User> CreateUser(User user);

    Task<User> UpdateUser(User user);

    Task<User> DeleteUser(User user);
    Task<User?> GetUserByPropertyAndValue(string property, string value);
    
    

}