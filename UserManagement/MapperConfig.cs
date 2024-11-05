using AutoMapper;
using Entity;
using UserManagement.DTOs.ConservationDto;
using UserManagement.DTOs.CourtOwnerDto.ViewDto;
using UserManagement.DTOs.FriendDto;
using UserManagement.DTOs.SportUserDto;
using UserManagement.DTOs.TeamDto;
using UserManagement.DTOs.UserDto;
using UserManagement.DTOs.UserDto.ViewDto;
using UserManagement.DTOs.UserTeamDto;
using SportViewDto = UserManagement.DTOs.SportDto.SportViewDto;

namespace UserManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<User, UserViewDto>()
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.ToString("yyyy-MM-dd"))); // Example for `DateOnly`
        CreateMap<UserViewDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore());
        
        CreateMap<UserSport, UserSportViewDto>().ReverseMap();
        CreateMap<Sport, SportViewDto>().ReverseMap();
        
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
        
        //
        CreateMap<UserTeam, UserTeamViewDto>().ReverseMap();
        
        CreateMap<Team, TeamViewDto>().ReverseMap();
        
        CreateMap<Conservation, ConservationViewDto>().ReverseMap();
        
        CreateMap<Sport, SportViewDto>().ReverseMap();

        CreateMap<Friendship, FriendshipViewDto>().ReverseMap();

        CreateMap<CourtOwnerViewDto, CourtOwner>().ReverseMap();



    }
}