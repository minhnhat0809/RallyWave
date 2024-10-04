using AutoMapper;
using Entity;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;
using MatchManagement.Enum;
using MatchType = MatchManagement.Enum.MatchType;

namespace MatchManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Match, MatchViewDto>()
            .ForMember(dest => dest.MatchType,
                opt => opt.MapFrom(src => System.Enum.GetName(typeof(MatchType), src.MatchType)))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => System.Enum.GetName(typeof(MatchStatus), src.Status!)))
            .ForMember(dest => dest.Mode,
                opt => opt.MapFrom(src => System.Enum.GetName(typeof(Mode), src.Mode)))
            .ForMember(dest => dest.MinLevel,
                opt => opt.MapFrom(src => System.Enum.GetName(typeof(SportLevel), src.MinLevel ?? 0)))
            .ForMember(dest => dest.MaxLevel,
                opt => opt.MapFrom(src => System.Enum.GetName(typeof(SportLevel), src.MaxLevel ?? 4)));
        
        

        CreateMap<MatchCreateDto, Match>();

        CreateMap<MatchUpdateDto, Match>();
    }
}