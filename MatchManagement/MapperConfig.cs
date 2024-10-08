using AutoMapper;
using Entity;
using MatchManagement.DTOs.MatchDto;
using MatchManagement.DTOs.MatchDto.ViewDto;

namespace MatchManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Match, MatchViewDto>();
        
        CreateMap<MatchCreateDto, Match>();

        CreateMap<MatchUpdateDto, Match>();
    }
}