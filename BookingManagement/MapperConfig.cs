using AutoMapper;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Booking, BookingViewDto>();
        CreateMap<BookingCreateDto, Booking>();
    }
}