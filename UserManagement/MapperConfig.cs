using AutoMapper;
using Entity;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;

namespace UserManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>()
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToString("yyyy-MM-dd"))); // Example for `DateOnly`

        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}