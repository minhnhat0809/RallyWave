using AutoMapper;
using Entity;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>().ReverseMap();
        
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}