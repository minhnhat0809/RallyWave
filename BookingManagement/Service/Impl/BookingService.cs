using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.Enum;
using BookingManagement.Repository;
using BookingManagement.Ultility;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate) : IBookingService
{
    
    public async Task<ResponseDto> GetBookings(string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            List<Booking>? bookings;
            
            if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
            {
                /*bookings = await unitOfWork.bookingRepo.FindAllAsync(b => b.Court,
                    b => b.Match, b => b.User, b => b.PaymentDetail);*/

                bookings = await unitOfWork.bookingRepo.FindAllAsync();
            }
            else
            {
                bookings = await unitOfWork.bookingRepo.GetBookings(filterField, filterValue);
            }
            
            
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
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            var booking = await unitOfWork.bookingRepo.GetBookingById(bookingId);
            if (booking == null)
            {
                responseDto.Message = "There are no bookings with this id";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
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
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
            responseDto.Message = e.Message;
        }

        return responseDto;
    }

    public async Task<ResponseDto> CreateBooking(BookingCreateDto bookingCreateDto)
    {
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status201Created);
        try
        {
            responseDto = await ValidateForCreating(bookingCreateDto);
            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }

            var booking = mapper.Map<Booking>(bookingCreateDto);
            booking.CreateAt = DateTime.Now;
            booking.Status = (sbyte) BookingStatus.Pending;
            
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
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
                return responseDto;
            }

            responseDto = await ValidateForUpdating(id, bookingUpdateDto);

            if (responseDto.IsSucceed == false)
            {
                return responseDto;
            }
            
            booking = mapper.Map<Booking>(bookingUpdateDto);
            
            await unitOfWork.bookingRepo.UpdateBooking(booking);
            responseDto.Message = "Update successfully!";
            
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
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
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                booking.Status = (sbyte) BookingStatus.Canceled;
                await unitOfWork.bookingRepo.DeleteBooking(booking);
                responseDto.Message = "Delete successfully!";
            }
        }
        catch (Exception e)
        {
            responseDto.IsSucceed = false;
            responseDto.Message = e.Message;
            responseDto.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return responseDto;
    }

    private async Task<ResponseDto> ValidateForCreating(BookingCreateDto bookingCreateDto)
    {
        var response = new ResponseDto(null, "", true, 200);

        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        var timeNow = TimeOnly.FromDateTime(DateTime.Now);
        
        // validate user and court
        if (bookingCreateDto is { MatchId: not null, UserId: not null })
        {
            response.IsSucceed = false;
            response.Message = "Match and user cannot be existed parallel";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate match
        if (bookingCreateDto.MatchId.HasValue)
        {
            var match = await unitOfWork.matchRepo.GetByIdAsync(bookingCreateDto.MatchId.Value);
            if (match == null)
            {
                response.IsSucceed = false;
                response.Message = "There are no matches with this id";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        
        // validate user
        if (bookingCreateDto.UserId.HasValue)
        {
            var match = await unitOfWork.matchRepo.GetByIdAsync(bookingCreateDto.UserId.Value);
            if (match == null)
            {
                response.IsSucceed = false;
                response.Message = "There are no users with this id";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        
        // validate court
        if (!bookingCreateDto.CourtId.HasValue)
        {
            response.IsSucceed = false;
            response.Message = "Court Id cannot be null.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        var court = await unitOfWork.courtRepo.GetCourtById(bookingCreateDto.CourtId.Value);
        if (court == null)
        {
            response.IsSucceed = false;
            response.Message = "There are no courts with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate slot
        if (!bookingCreateDto.SlotId.HasValue)
        {
            response.IsSucceed = false;
            response.Message = "Slot Id cannot be null.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var slot = await unitOfWork.slotRepo.GetByIdAsync(bookingCreateDto.SlotId);
        if (slot == null)
        {
            response.IsSucceed = false;
            response.Message = "There are no slots with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        // validate date and time start
        switch (bookingCreateDto.Date.CompareTo(today))
        {
            case < 0:
                response.IsSucceed = false;
                response.Message = "Reserved date is in the past.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            case 0:
                if (bookingCreateDto.TimeStart.CompareTo(timeNow) < 0.5)
                {
                    response.IsSucceed = false;
                    response.Message = "Reserved time is too closed.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                break;
        }

        // validate time start and time end
        if (bookingCreateDto.TimeStart >= bookingCreateDto.TimeEnd)
        {
            response.IsSucceed = false;
            response.Message = "Time start is bigger than time end.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate overlap time
        response = await CheckCourtAvailableForCreating(slot, bookingCreateDto.Date, bookingCreateDto.TimeStart,
            bookingCreateDto.TimeEnd);

        return response;
    }
    
    private async Task<ResponseDto> ValidateForUpdating(int id, BookingUpdateDto bookingUpdateDto)
    {
        var response = new ResponseDto(null, "", true, 200);

        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        var timeNow = TimeOnly.FromDateTime(DateTime.Now);
        
        // validate user and court
        if (bookingUpdateDto is { MatchId: not null, UserId: not null })
        {
            response.IsSucceed = false;
            response.Message = "Match and user cannot be existed parallel";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate match
        if (bookingUpdateDto.MatchId.HasValue)
        {
            var match = await unitOfWork.matchRepo.GetByIdAsync(bookingUpdateDto.MatchId.Value);
            if (match == null)
            {
                response.IsSucceed = false;
                response.Message = "There are no matches with this id";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        
        // validate user
        if (bookingUpdateDto.UserId.HasValue)
        {
            var match = await unitOfWork.matchRepo.GetByIdAsync(bookingUpdateDto.UserId.Value);
            if (match == null)
            {
                response.IsSucceed = false;
                response.Message = "There are no users with this id";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            }
        }
        
        // validate court
        if (!bookingUpdateDto.CourtId.HasValue)
        {
            response.IsSucceed = false;
            response.Message = "Court Id cannot be null.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        var court = await unitOfWork.courtRepo.GetCourtById(bookingUpdateDto.CourtId.Value);
        if (court == null)
        {
            response.IsSucceed = false;
            response.Message = "There are no courts with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate slot
        if (!bookingUpdateDto.SlotId.HasValue)
        {
            response.IsSucceed = false;
            response.Message = "Slot Id cannot be null.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var slot = await unitOfWork.slotRepo.GetByIdAsync(bookingUpdateDto.SlotId);
        if (slot == null)
        {
            response.IsSucceed = false;
            response.Message = "There are no slots with this id";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        // validate date and time start
        switch (bookingUpdateDto.Date.CompareTo(today))
        {
            case < 0:
                response.IsSucceed = false;
                response.Message = "Reserved date is in the past.";
                response.StatusCode = StatusCodes.Status400BadRequest;
                return response;
            case 0:
                if (bookingUpdateDto.TimeStart.CompareTo(timeNow) < 0.5)
                {
                    response.IsSucceed = false;
                    response.Message = "Reserved time is too closed.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }
                break;
        }

        // validate time start and time end
        if (bookingUpdateDto.TimeStart >= bookingUpdateDto.TimeEnd)
        {
            response.IsSucceed = false;
            response.Message = "Time start is bigger than time end.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        // validate overlap time
        response = await CheckCourtAvailableForUpdating(id, slot, bookingUpdateDto.Date, bookingUpdateDto.TimeStart,
            bookingUpdateDto.TimeEnd);

        return response;
    }

    private async Task<ResponseDto> CheckCourtAvailableForCreating(Slot slot, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
    {
        var response = new ResponseDto(null, "", true, 200);
        
        // compare time start and time end with time of slot
        if (slot.TimeStart.CompareTo(timeStart) > 0 || slot.TimeEnd.CompareTo(timeEnd) < 0)
        {
            response.IsSucceed = false;
            response.Message = "Time of booking is not in rang time of slot";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var bookings = await unitOfWork.bookingRepo.FindByConditionAsync(b => b.SlotId != null && b.Date.Equals(date) && b.SlotId.Value.Equals(slot.SlotId));
        
        // Check if there is any time conflict with existing bookings
        if (!bookings.Any(b => timeStart < b.TimeEnd && timeEnd > b.TimeStart)) return response;
        
        response.IsSucceed = false;
        response.Message = "The requested time overlaps with an existing booking.";
        response.StatusCode = StatusCodes.Status400BadRequest;
        return response;
    }
    
    private async Task<ResponseDto> CheckCourtAvailableForUpdating(int id, Slot slot, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
    {
        var response = new ResponseDto(null, "", true, 200);
        
        // compare time start and time end with time of slot
        if (slot.TimeStart.CompareTo(timeStart) > 0 || slot.TimeEnd.CompareTo(timeEnd) < 0)
        {
            response.IsSucceed = false;
            response.Message = "Time of booking is not in rang time of slot";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        var bookings = await unitOfWork.bookingRepo.FindByConditionAsync(b => b.SlotId != null && b.Date.Equals(date) && 
                                                                              b.SlotId.Value.Equals(slot.SlotId) &&
                                                                              b.BookingId != id);
        
        // Check if there is any time conflict with existing bookings
        if (!bookings.Any(booking => timeStart < booking.TimeEnd && timeEnd > booking.TimeStart)) return response;
        
        response.IsSucceed = false;
        response.Message = "The requested time overlaps with an existing booking.";
        response.StatusCode = StatusCodes.Status400BadRequest;
        return response;
    }

    private static List<Booking>? Paging(List<Booking>? bookings, int pageNumber, int pageSize)
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

    private static List<Booking>? Sort(List<Booking>? bookings, string? sortField, string? sortValue)
    {
        if (bookings == null || bookings.Count == 0 || string.IsNullOrEmpty(sortField) || 
            string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
        {
            return bookings;
        }

        bookings = sortField.ToLower() switch
        {
            "date" => sortValue.Equals("asc")
                ? bookings.OrderBy(b => b.Date).ToList()
                : bookings.OrderByDescending(b => b.Date).ToList(),
            "timestart" => sortValue.Equals("asc")
                ? bookings.OrderBy(b => b.TimeStart).ToList()
                : bookings.OrderByDescending(b => b.TimeStart).ToList(),
            "timeend" => sortValue.Equals("asc")
                ? bookings.OrderBy(b => b.TimeEnd).ToList()
                : bookings.OrderByDescending(b => b.TimeEnd).ToList(),
            "status" => sortValue.Equals("asc")
                ? bookings.OrderBy(b => b.Status).ToList()
                : bookings.OrderByDescending(b => b.Status).ToList(),
            "createat" => sortValue.Equals("asc")
                ? bookings.OrderBy(b => b.CreateAt).ToList()
                : bookings.OrderByDescending(b => b.CreateAt).ToList(),
            _ => bookings
        };

        return bookings;
    }
    
}