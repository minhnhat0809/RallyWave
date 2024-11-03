using AutoMapper;
using ChattingManagement.DTOs.ConservationDto;
using ChattingManagement.DTOs.ConservationDto.ViewDto;
using Entity;

namespace ChattingManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Conservation, ConservationViewDto>().ReverseMap();

        CreateMap<ConservationCreateDto, Conservation>();

        CreateMap<ConservationUpdateDto, Conservation>();
    }
}