using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.Repository;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
{
    
    public async Task<ResponseDto> GetBookings(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var bookings = await unitOfWork.bookingRepo.GetBookings(filterField, filterValue);

            bookings = Sort(bookings, sortField, sortValue);

            bookings = Paging(bookings, pageNumber, pageSize);
            
            responseDto.Result = mapper.Map<List<BookingViewDto>>(bookings);
            responseDto.Message = "Get successfully!";
        }
        catch (Exception e)
        {
            responseDto.Message = e.Message;
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
        }
        return responseDto;
    }

    public async Task<ResponseDto> GetBookingById(int bookingId)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var booking = await unitOfWork.bookingRepo.GetBookingById(bookingId);
            if (booking == null)
            {
                responseDto.Message = "There are no bookings with this id";
            }
            else
            {
                responseDto.Result = booking;
                responseDto.Message = "Get successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.StatusCode = 500;
            responseDto.Message = e.Message;
        }

        return responseDto;
    }

    public async Task<ResponseDto> CreateBooking(BookingCreateDto bookingCreateDto)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            responseDto = Validate(bookingCreateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            var booking = mapper.Map<Booking>(bookingCreateDto);
            booking.CreateAt = DateTime.Now;
            
            await unitOfWork.bookingRepo.CreateBooking(booking);
            
            responseDto.Message = "Create successfully!";
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto; 
    }

    public async Task<ResponseDto> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var booking = await unitOfWork.bookingRepo.GetBookingById(id);
            if (booking == null)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = "There are no bookings with this id";
                responseDto.StatusCode = 401;
            }
            else
            {
                booking = mapper.Map<Booking>(bookingUpdateDto);
                await unitOfWork.bookingRepo.UpdateBooking(booking);

                responseDto.Message = "Update successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto; 
    }

    public async Task<ResponseDto> DeleteBooking(int id)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            var booking = await unitOfWork.bookingRepo.GetBookingById(id);
            if (booking == null)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = "There are no bookings with this id";
                responseDto.StatusCode = 401;
            }
            else
            {
                await unitOfWork.bookingRepo.DeleteAsync(booking);
                responseDto.Message = "Delete successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = 500;
        }

        return responseDto;
    }

    private ResponseDto Validate(BookingCreateDto bookingCreateDto)
    {
        var response = new ResponseDto(null, "", true, 200);

        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        var timeNow = TimeOnly.FromDateTime(DateTime.Now);
        
        // validate court
        if (!bookingCreateDto.CourtId.HasValue)
        {
            response.IsSucceed = false;
            response.Message = "Court Id cannot be null.";
            return response;
        }

        //validate date and time start
        switch (bookingCreateDto.Date.CompareTo(today))
        {
            case < 0:
                response.IsSucceed = false;
                response.Message = "Reserved date is in the past.";
                return response;
            case 0:
                if (bookingCreateDto.TimeStart.CompareTo(timeNow) < 0.5)
                {
                    response.IsSucceed = false;
                    response.Message = "Reserved time is too closed.";
                    return response;
                }
                break;
        }

        // validate time start and time end
        if (bookingCreateDto.TimeStart >= bookingCreateDto.TimeEnd)
        {
            response.IsSucceed = false;
            response.Message = "Time start is bigger than time end.";
            return response;
        }

        return response;
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