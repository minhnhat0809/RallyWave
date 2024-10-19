using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.Repository;

public interface IUserRepo : IRepositoryBase<User>
{
    Task<List<User>> GetUsers(string? filterField, string? filterValue);

    Task<User?> GetUserById(int userId);

    Task<User> CreateUser(User user);

    Task<User> UpdateUser(User user);

    Task<User> DeleteUser(User user);
}