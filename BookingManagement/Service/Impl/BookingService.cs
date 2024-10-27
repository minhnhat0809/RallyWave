using System.Linq.Expressions;
using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.Repository;
using BookingManagement.Ultility;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper, Validate validate, ListExtensions listExtensions) : IBookingService
{
    private class MatchSlot (TimeOnly timeStart, TimeOnly timeEnd, double cost)
    {
        public TimeOnly TimeStart { get; set; } = timeStart;
        public TimeOnly TimeEnd { get; set; } = timeEnd;
        public double Cost { get; set; } = cost;
    }
    
    public async Task<ResponseDto> GetBookings(string? subject, int? subjectId, string? filterField, string? filterValue, string? sortField, string sortValue, int pageNumber,
        int pageSize)
    {
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
        try
        {
            List<BookingsViewDto>? bookings;

            int total;
            
            if (validate.IsEmptyOrWhiteSpace(filterField) || validate.IsEmptyOrWhiteSpace(filterValue))
            {
                Expression<Func<Booking, bool>> basePredicate = b => true;
                
                if (!validate.IsEmptyOrWhiteSpace(subject) || subjectId.HasValue)
                {
                    basePredicate = subject!.ToLower() switch
                    {
                        "user" => b => b.UserId.HasValue && b.UserId.Value == subjectId!.Value,
                        "court" => b => b.CourtId == subjectId!.Value,
                        _ => throw new ArgumentException($"Unknown subject '{subject}'")
                    };
                    
                    
                }

                bookings = await unitOfWork.BookingRepo.FindByConditionWithPagingAsync(
                    basePredicate, 
                    b => new BookingsViewDto(b.BookingId, b.Date, b.TimeStart, b.TimeEnd, b.Cost, b.Status),
                    pageSize, pageSize);

                total = await unitOfWork.BookingRepo.CountByConditionAsync(basePredicate);

            }
            else
            {
                var listResponse = await unitOfWork.BookingRepo.GetBookings(subject, subjectId, filterField!, filterValue!, pageNumber, pageSize);
                
                bookings = listResponse.Data;
                
                total = listResponse.TotalCount;
            }
            
            
            
            bookings = Sort(bookings, sortField, sortValue);
            
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
            var responseCourtSlot = await ValidateForCreating(bookingCreateDto);
            if (responseDto.IsSucceed == false)
            {
                responseDto.Message = responseCourtSlot.Message;
                responseDto.IsSucceed = false;
                responseDto.StatusCode = responseCourtSlot.StatusCode;
                return responseDto;
            }

            var booking = mapper.Map<Booking>(bookingCreateDto);
            booking.CreateAt = DateTime.Now;
            booking.Cost = responseCourtSlot.Cost!.Value;
            
            await unitOfWork.BookingRepo.CreateBooking(booking);
            
            responseDto.Result = mapper.Map<BookingViewDto>(booking);
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

            var responseCourtSlot = await ValidateForUpdating(id, bookingUpdateDto);

            if (responseDto.IsSucceed == false)
            {
                responseDto.Message = responseCourtSlot.Message;
                responseDto.IsSucceed = false;
                responseDto.StatusCode = responseCourtSlot.StatusCode;
                return responseDto;
            }
            
            booking = mapper.Map<Booking>(bookingUpdateDto);
            booking.CreateAt = DateTime.Now;
            booking.Cost = responseCourtSlot.Cost!.Value;
            
            await unitOfWork.BookingRepo.UpdateBooking(booking);

            responseDto.Result = mapper.Map<BookingViewDto>(booking);
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
        var responseDto = new ResponseDto(null, "", true, StatusCodes.Status200OK);
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

    private async Task<ResponseCourtSlotDto> ValidateForCreating(BookingCreateDto bookingCreateDto)
    {
        var today = DateOnly.FromDateTime(DateTime.Now.Date);
        var timeNow = TimeOnly.FromDateTime(DateTime.Now);

        // Validate that both MatchId and UserId are not provided at the same time
        if (bookingCreateDto is { MatchId: not null, UserId: not null })
        {
            return new ResponseCourtSlotDto(null, "Match and user cannot be specified simultaneously.", false, StatusCodes.Status400BadRequest);
        }

        // Validate MatchId
        if (bookingCreateDto.MatchId.HasValue)
        {
            var match = await unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == bookingCreateDto.MatchId.Value, 
                m => new { m.MatchId, m.MatchName });
            
            if (match == null)
            {
             return new ResponseCourtSlotDto(null, "Match not found.", false, StatusCodes.Status404NotFound);
            }
        }

        // Validate UserId
        if (bookingCreateDto.UserId.HasValue)
        {
            var user = await unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == bookingCreateDto.UserId.Value, 
                u => new { u.UserId, u.UserName });
            
            if (user == null)
            {
                return new ResponseCourtSlotDto(null, "User not found.", false, StatusCodes.Status404NotFound);
            }
        }
        
        //Check court in database
        var court = await unitOfWork.CourtRepo.GetCourtById(bookingCreateDto.CourtId);
        if (court == null)
        {
            return new ResponseCourtSlotDto(null, "Court not found.", false, StatusCodes.Status404NotFound);
        }

        // Validate booking date and time
        var dateComparison = bookingCreateDto.Date.CompareTo(today);
        switch (dateComparison)
        {
            case < 0:
                return new ResponseCourtSlotDto(null, "Reserved date is in the past.", false, StatusCodes.Status400BadRequest);
            case 0 when bookingCreateDto.TimeStart.CompareTo(timeNow) <= 0:
                return new ResponseCourtSlotDto(null, "Reserved time is too close.", false, StatusCodes.Status400BadRequest);
        }

        // Validate time range (TimeStart < TimeEnd)
        if (bookingCreateDto.TimeStart >= bookingCreateDto.TimeEnd)
        {
            return new ResponseCourtSlotDto(null, "Start time must be earlier than end time.", false, StatusCodes.Status400BadRequest);
        }

        //Validate time range is in any slots
        var response = await ValidateSlotAndCalculateCost(bookingCreateDto.CourtId, bookingCreateDto.TimeStart, bookingCreateDto.TimeEnd);
        if (!response.IsSucceed)
        {
            return response;
        }

        //Store cost in local variable
        double total = 0;
        if(response.Cost.HasValue){total = response.Cost.Value;}

        // Validate time overlap with existing slots
        response = await CheckCourtAvailableForCreating(bookingCreateDto.CourtId, bookingCreateDto.Date, bookingCreateDto.TimeStart, bookingCreateDto.TimeEnd);

        //Set cost back to return
        response.Cost = total;
        
        return response;
    }
    
    private async Task<ResponseCourtSlotDto> ValidateForUpdating(int id, BookingUpdateDto bookingUpdateDto)
    {
        var response = new ResponseCourtSlotDto(null, "", true, 200);

        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        var timeNow = TimeOnly.FromDateTime(DateTime.Now);
        
        // validate user and match
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
        var court = await unitOfWork.CourtRepo.GetCourtById(bookingUpdateDto.CourtId);
        if (court == null)
        {
            response.IsSucceed = false;
            response.Message = "There are no courts with this id";
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
        
        response = await ValidateSlotAndCalculateCost(bookingUpdateDto.CourtId, bookingUpdateDto.TimeStart, bookingUpdateDto.TimeEnd);
        
        //Validate time range is in any slots
        if (!response.IsSucceed)
        {
            return response;
        }
        
        //Store cost in local variable
        double total = 0;
        if(response.Cost.HasValue){total = response.Cost.Value;}
        
        // validate overlap time
        response = await CheckCourtAvailableForUpdating(bookingUpdateDto.CourtId, id, bookingUpdateDto.Date, bookingUpdateDto.TimeStart,
            bookingUpdateDto.TimeEnd);

        //Set cost back to return
        response.Cost = total;
        
        return response;
    }

    private async Task<ResponseCourtSlotDto> CheckCourtAvailableForCreating(int courtId, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
    {
        var response = new ResponseCourtSlotDto(null, "", true, 200);
        
        //get booking that overlaps
        var overlapBooking = await unitOfWork.BookingRepo
            .AnyAsync(b => b.CourtId == courtId &&
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
    
    private async Task<ResponseCourtSlotDto> CheckCourtAvailableForUpdating(int courtId, int id, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
    {
        var response = new ResponseCourtSlotDto(null, "", true, 200);

        //get booking that overlaps
        var overlapBooking = await unitOfWork.BookingRepo
            .AnyAsync(b => b.CourtId == courtId &&
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

    private async Task<ResponseCourtSlotDto> ValidateSlotAndCalculateCost(int courtId, TimeOnly timeStart, TimeOnly timeEnd)
    {
        var response = new ResponseCourtSlotDto(null, "", true, 200);

        // Fetch the court slots for the given court
        var courtSlots = await unitOfWork.SlotRepo.FindByConditionAsync(s => s.CourtId == courtId, 
            s => new MatchSlot(
                s.TimeStart,
                s.TimeEnd,
                s.Cost
                ));

        if (courtSlots.Count == 0)
        {
            response.IsSucceed = false;
            response.Message = "No slots available for this court.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        //Find slots that match or overlap with the requested timeStart and timeEnd
        var matchingSlots = courtSlots
            .Where(s => s.TimeStart <= timeEnd && s.TimeEnd >= timeStart)
            .OrderBy(s => s.TimeStart)
            .ToList();

        //Check if the time range is fully covered
        if (matchingSlots.Count == 0 || matchingSlots.First().TimeStart > timeStart || matchingSlots.Last().TimeEnd < timeEnd)
        {
            response.IsSucceed = false;
            response.Message = "Time range is not fully covered by available slots.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }

        //Ensure there are no gaps between the slots
        for (var i = 0; i < matchingSlots.Count - 1; i++)
        {
            if (matchingSlots[i].TimeEnd == matchingSlots[i + 1].TimeStart) continue;
            response.IsSucceed = false;
            response.Message = "There is a gap between the available slots.";
            response.StatusCode = StatusCodes.Status400BadRequest;
            return response;
        }
        
        var totalCost = (from slot in matchingSlots 
                let overlapStart = timeStart > slot.TimeStart ? timeStart : slot.TimeStart 
                let overlapEnd = timeEnd < slot.TimeEnd ? timeEnd : slot.TimeEnd 
                let overlapDuration = (overlapEnd.ToTimeSpan() - overlapStart.ToTimeSpan()).TotalHours 
                select overlapDuration * slot.Cost)
            .Sum();

        response.Cost = totalCost;
        
        return response;
    }
}