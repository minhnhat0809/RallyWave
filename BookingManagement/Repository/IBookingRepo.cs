﻿using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement.Repository;

public interface IBookingRepo : IRepositoryBase<Booking>
{
    Task<ResponseListDto<BookingsViewDto>> GetBookings(
        string? subject, int? subjectId,
        BookingFilterDto? bookingFilterDto,
        string? sortField, string sortValue,
        int pageNumber, int pageSize);
}