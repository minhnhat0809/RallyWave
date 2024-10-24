using AutoMapper;
using Entity;
using UserManagement.DTOs.ConservationDto;
using UserManagement.DTOs.SportDto;
using UserManagement.DTOs.TeamDto;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.DTOs.UserTeamDto;

namespace UserManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>()
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToString("yyyy-MM-dd"))); // Example for `DateOnly`
        CreateMap<UserViewDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
        
        CreateMap<UserTeam, UserTeamViewDto>().ReverseMap();
        
        CreateMap<Team, TeamViewDto>().ReverseMap();
        CreateMap<TeamCreateDto, Team>();
        CreateMap<TeamUpdateDto, Team>();
        
        
        CreateMap<Conservation, ConservationViewDto>().ReverseMap();
        
        CreateMap<Sport, SportViewDto>().ReverseMap();
    }
}