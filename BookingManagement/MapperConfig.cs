﻿using AutoMapper;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.DTOs.CourtDto.ViewDto;
using BookingManagement.DTOs.MatchDto.ViewDto;
using BookingManagement.DTOs.PaymentDto.ViewDto;
using BookingManagement.DTOs.UserDto.ViewDto;
using Entity;

namespace BookingManagement;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Booking, BookingViewDto>();
        
        CreateMap<BookingCreateDto, Booking>();

        CreateMap<User, UserViewDto>();

        CreateMap<Match, MatchViewDto>();

        CreateMap<Court, CourtViewDto>();

        CreateMap<PaymentDetail, PaymentDetailViewDto>(); 
    }
}