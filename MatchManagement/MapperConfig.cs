using AutoMapper;
using Entity;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Match, MatchViewDto>()
            .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.SportName));
        
        CreateMap<MatchCreateDto, Match>();

        CreateMap<MatchUpdateDto, Match>()
            .ForMember(dest => dest.CreateBy, opt => opt.Ignore());
    }
}