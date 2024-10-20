using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.DTOs.CourtSlotDto.CourtSlotViewDto;
using BookingManagement.Repository;
using BookingManagement.Ultility;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions) : IBookingService
{
    
    public async Task<ResponseDto> GetBookings(int userId, string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, 200);
        try
        {
            List<BookingsViewDto>? bookings;

            int total;
            
            if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
            {
                bookings = await unitOfWork.BookingRepo.FindByConditionAsync(b => b.UserId.HasValue && b.UserId.Value == userId,
                    b => new BookingsViewDto(b.BookingId,
                        b.Date,
                        b.TimeStart,
                        b.TimeEnd,
                        b.Cost,
                        b.Status
                    ));

                total = bookings.Count;
            }
            else
            {
                bookings = await unitOfWork.BookingRepo.GetBookings(userId, filterField, filterValue);
                total = bookings.Count;
            }
            
            
            
            bookings = Sort(bookings, sortField, sortValue);

            bookings = listExtensions.Paging(bookings, pageNumber, pageSize);
            
            responseDto.Result = new { bookings, total};
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
            var booking = await unitOfWork.BookingRepo.GetBookingById(bookingId);
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
            
            await unitOfWork.BookingRepo.CreateBooking(booking);
            
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
            var booking = await unitOfWork.BookingRepo.GetBookingById(id);
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
            
            await unitOfWork.BookingRepo.UpdateBooking(booking);
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
            var booking = await unitOfWork.BookingRepo.GetBookingById(id);
            if (booking == null)
            {
                responseDto.IsSucceed = false;
                responseDto.Message = "There are no bookings with this id";
                responseDto.StatusCode = StatusCodes.Status400BadRequest;
            }
            else
            {
                booking.Status = 3;
                await unitOfWork.BookingRepo.DeleteBooking(booking);
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
        var today = DateOnly.FromDateTime(DateTime.Now.Date);
        var timeNow = TimeOnly.FromDateTime(DateTime.Now);

        // Validate that both MatchId and UserId are not provided at the same time
        if (bookingCreateDto is { MatchId: not null, UserId: not null })
        {
            return new ResponseDto(null, "Match and user cannot be specified simultaneously.", false, StatusCodes.Status400BadRequest);
        }

        // Validate MatchId
        if (bookingCreateDto.MatchId.HasValue)
        {
            var match = await unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == bookingCreateDto.MatchId.Value, 
                m => new { m.MatchId, m.MatchName });
            
            if (match == null)
            {
             return new ResponseDto(null, "Match not found.", false, StatusCodes.Status404NotFound);
            }
        }

        // Validate UserId
        if (bookingCreateDto.UserId.HasValue)
        {
            var user = await unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == bookingCreateDto.UserId.Value, 
                u => new { u.UserId, u.UserName });
            
            if (user == null)
            {
                return new ResponseDto(null, "User not found.", false, StatusCodes.Status404NotFound);
            }
        }

        // Validate CourtId
        if (!bookingCreateDto.CourtId.HasValue)
        {
            return new ResponseDto(null, "Court Id cannot be null.", false, StatusCodes.Status400BadRequest);
        }

        var court = await unitOfWork.CourtRepo.GetCourtById(bookingCreateDto.CourtId.Value);
        if (court == null)
        {
            return new ResponseDto(null, "Court not found.", false, StatusCodes.Status404NotFound);
        }

        // Validate SlotId
        if (!bookingCreateDto.SlotId.HasValue)
        {
            return new ResponseDto(null, "Slot Id cannot be null.", false, StatusCodes.Status400BadRequest);
        }

        var slot = await unitOfWork.SlotRepo.GetByConditionAsync(s => s.SlotId == bookingCreateDto.SlotId.Value,
            s => new SlotValidateDto(s.SlotId, s.TimeStart, s.TimeEnd));

        if (slot == null)
        {
            return new ResponseDto(null, "Slot not found.", false, StatusCodes.Status404NotFound);
        }

        // Validate booking date and time
        var dateComparison = bookingCreateDto.Date.CompareTo(today);
        switch (dateComparison)
        {
            case < 0:
                return new ResponseDto(null, "Reserved date is in the past.", false, StatusCodes.Status400BadRequest);
            case 0 when bookingCreateDto.TimeStart.CompareTo(timeNow) <= 0:
                return new ResponseDto(null, "Reserved time is too close.", false, StatusCodes.Status400BadRequest);
        }

        // Validate time range (TimeStart < TimeEnd)
        if (bookingCreateDto.TimeStart >= bookingCreateDto.TimeEnd)
        {
            return new ResponseDto(null, "Start time must be earlier than end time.", false, StatusCodes.Status400BadRequest);
        }

        // Validate time overlap with existing slots
        var response = await CheckCourtAvailableForCreating(slot, bookingCreateDto.Date, bookingCreateDto.TimeStart, bookingCreateDto.TimeEnd);

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
            var match = await unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == bookingUpdateDto.MatchId.Value, 
                m => new {m.MatchId, m.MatchName}, null);
            
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
            var match = await unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == bookingUpdateDto.UserId.Value, u => new {u.UserId});
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
        
        var court = await unitOfWork.CourtRepo.GetCourtById(bookingUpdateDto.CourtId.Value);
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

        var slot = await unitOfWork.SlotRepo.GetByConditionAsync(s => s.SlotId == bookingUpdateDto.SlotId, 
            s => new SlotValidateDto
            (
                s.SlotId,
                s.TimeStart,
                s.TimeEnd
            ));
        
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

    private async Task<ResponseDto> CheckCourtAvailableForCreating(SlotValidateDto slot, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
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
        
        //get booking that overlaps
        var overlapBooking = await unitOfWork.BookingRepo
            .AnyAsync(b =>
                           b.Date.Equals(date) &&
                           b.TimeStart < timeEnd &&
                           b.TimeEnd > timeStart);
        
        // Check if there is any time conflict with existing bookings
        if (overlapBooking) return response;
        
        response.IsSucceed = false;
        response.Message = "The requested time overlaps with an existing booking.";
        response.StatusCode = StatusCodes.Status400BadRequest;
        return response;
    }
    
    private async Task<ResponseDto> CheckCourtAvailableForUpdating(int id, SlotValidateDto slot, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
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

        //get booking that overlaps
        var overlapBooking = await unitOfWork.BookingRepo
            .AnyAsync(b =>
                           b.BookingId !=  id &&
                           b.Date.Equals(date) &&
                           b.TimeStart < timeEnd &&
                           b.TimeEnd > timeStart);
        
        // Check if there is any time conflict with existing bookings
        if (overlapBooking) return response;
        
        response.IsSucceed = false;
        response.Message = "The requested time overlaps with an existing booking.";
        response.StatusCode = StatusCodes.Status400BadRequest;
        return response;
    }

    private List<BookingsViewDto>? Sort(List<BookingsViewDto>? bookings, string? sortField, string? sortValue)
    {
        if (bookings == null || bookings.Count == 0 || string.IsNullOrEmpty(sortField) || 
            string.IsNullOrEmpty(sortValue) || string.IsNullOrWhiteSpace(sortField) || string.IsNullOrWhiteSpace(sortValue))
        {
            return bookings;
        }

        bookings = sortField.ToLower() switch
        {
            "date" => sortValue.Equals("asc")
                ? listExtensions.Sort(bookings, b => b.Date, true)
                : listExtensions.Sort(bookings, b => b.Date, false),
            "timestart" => sortValue.Equals("asc")
                ? listExtensions.Sort(bookings, b => b.TimeStart, true)
                : listExtensions.Sort(bookings, b => b.TimeStart, false),
            "timeend" => sortValue.Equals("asc")
                ? listExtensions.Sort(bookings, b => b.TimeEnd, true)
                : listExtensions.Sort(bookings, b => b.TimeEnd, false),
            "status" => sortValue.Equals("asc")
                ? listExtensions.Sort(bookings, b => b.Status, true)
                : listExtensions.Sort(bookings, b => b.Status, false),
            _ => bookings
        };

        return bookings;
    }
    
}