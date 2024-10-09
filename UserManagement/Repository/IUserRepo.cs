using Entity;
using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.Repository;

public interface IUserRepo : IRepositoryBase<User>
{
    Task<List<UserViewDto>> GetUsers(string? filterField, string? filterValue);

    Task<UserViewDto?> GetUserById(int userId);

    Task<UserViewDto> CreateUser(User user);

    Task<UserViewDto> UpdateUser(User user);

    Task<UserViewDto> DeleteUser(User user);
}