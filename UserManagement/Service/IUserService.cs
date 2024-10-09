using UserManagement.DTOs;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement.Service;

public interface IUserService
{
    Task<ResponseDto> GetUser(string? filterField,
        string? filterValue,
        string? sortField,
        string sortValue,
        int pageNumber,
        int pageSize);

    Task<ResponseDto> GetUserById(int userId);

    Task<ResponseDto> CreateUser(UserCreateDto userCreateDto);

    Task<ResponseDto> UpdateUser(int id, UserUpdateDto userCreateDto);

    Task<ResponseDto> DeleteUser(int id);
}