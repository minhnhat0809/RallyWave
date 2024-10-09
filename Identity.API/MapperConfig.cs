using AutoMapper;
using Entity;
using Identity.API.BusinessObjects.UserViewModel;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace Identity.API;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>().ReverseMap();
         
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}