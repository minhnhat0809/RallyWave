using AutoMapper;
using CourtManagement.DTOs.CourtDto;
using CourtManagement.DTOs.CourtDto.ViewDto;
using Entity;

namespace CourtManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Court, CourtViewDto>();

        CreateMap<Court, CourtsViewDto>();

        CreateMap<CourtCreateDto, Court>();

        CreateMap<CourtUpdateDto, Court>();
    }
}