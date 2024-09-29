using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto.ViewDto;
using WebApplication1.Repository;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ResponseDto> GetBookings(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var bookings = await _unitOfWork.bookingRepo.GetBookings(filterField, filterValue);

            bookings = Sort(bookings, sortField, sortValue);

            bookings = Paging(bookings, pageNumber, pageSize);
            
            responseDto.Result = mapper.Map<List<BookingViewDto>>(bookings);
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }


    private List<Booking>? Paging(List<Booking>? bookings, int pageNumber, int pageSize)
    {
        if (bookings == null || bookings.Count == 0)
        {
            return bookings;
        }

        bookings = bookings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        
        return bookings;
    }

    private List<Booking>? Sort(List<Booking>? bookings, string? sortField, string? sortValue)
    {
        if (bookings == null || bookings.Count == 0 || string.IsNullOrEmpty(sortField))
        {
            return bookings;
        }

        switch (sortField.ToLower())
        {
            case "date":
                bookings = sortValue == "asc" ? bookings.OrderBy(b => b.Date).ToList() : bookings.OrderByDescending(b => b.Date).ToList();
                break;
            case "timestart":
                bookings = sortValue == "asc" ? bookings.OrderBy(b => b.TimeStart).ToList() : bookings.OrderByDescending(b => b.TimeStart).ToList();
                break;
            case "timeend":
                bookings = sortValue == "asc" ? bookings.OrderBy(b => b.TimeEnd).ToList() : bookings.OrderByDescending(b => b.TimeEnd).ToList();
                break;
            case "status":
                bookings = sortValue == "asc" ? bookings.OrderBy(b => b.Status).ToList() : bookings.OrderByDescending(b => b.Status).ToList();
                break;
            case "createat":
                bookings = sortValue == "asc" ? bookings.OrderBy(b => b.CreateAt).ToList() : bookings.OrderByDescending(b => b.CreateAt).ToList();
                break;
        }

        return bookings;
    }
    
}