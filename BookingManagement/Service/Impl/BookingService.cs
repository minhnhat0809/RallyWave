using AutoMapper;
using Entity;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto;
using BookingManagement.DTOs.BookingDto.ViewDto;
using BookingManagement.Enum;
using BookingManagement.Repository;

namespace BookingManagement.Service.Impl;

public class BookingService(IUnitOfWork unitOfWork, IMapper mapper) : IBookingService
{
    private class MatchSlot (TimeOnly timeStart, TimeOnly timeEnd, double cost)
    {
        public TimeOnly TimeStart { get; } = timeStart;
        public TimeOnly TimeEnd { get; } = timeEnd;
        public double Cost { get; } = cost;
    }
    
    public async Task<ResponseDto> GetBookings(string? subject, int? subjectId, BookingFilterDto? bookingFilterDto, 
        string? sortField, string sortValue, int pageNumber, int pageSize)
    {
        try
        {
            var listResponse = await unitOfWork.BookingRepo.GetBookings(subject, subjectId, bookingFilterDto, sortField, sortValue, pageNumber, pageSize); 
            
            var bookings = listResponse.Data;
                
            var total = listResponse.TotalCount;
            return new ResponseDto(new { bookings, total}, "Thành công", true, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto> GetBookingById(int bookingId)
    {
        try
        {
            var booking = await unitOfWork.BookingRepo.GetByConditionAsync(b => b.BookingId == bookingId, b => b);
            
            if (booking == null)
                return new ResponseDto(null, "Không có lượt đặt nào với id này", false, StatusCodes.Status404NotFound);
            
            var bookingDto = mapper.Map<BookingViewDto>(booking);
            
            return new ResponseDto(bookingDto, "Thành công", true, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto> CreateBooking(BookingCreateDto bookingCreateDto)
    {
        try
        {
            var response = await OverallValidate(bookingCreateDto.CourtId, bookingCreateDto.Date, bookingCreateDto.TimeStart,
                bookingCreateDto.TimeEnd, bookingCreateDto.MatchId, bookingCreateDto.UserId, null);

            if (response.IsSucceed == false) return response;

            var booking = mapper.Map<Booking>(bookingCreateDto);
            
            booking.CreateAt = DateTime.Now;
            booking.Cost = (double) response.Result!;
            booking.Status = (sbyte) BookingStatus.Pending;
            
            await unitOfWork.BookingRepo.CreateAsync(booking);
            
            var bookingDto = mapper.Map<BookingViewDto>(booking);
            return new ResponseDto(bookingDto, "Tạo thành công", true, StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto> UpdateBooking(int id, BookingUpdateDto bookingUpdateDto)
    {
        try
        {
            var booking = await unitOfWork.BookingRepo.GetByConditionAsync(b => b.BookingId == id, b => b);
            
            if (booking == null)
                return new ResponseDto(null, "Không có lượt đặt nào với id này", false, StatusCodes.Status404NotFound);

            if (booking.Date != bookingUpdateDto.Date || booking.TimeStart != bookingUpdateDto.TimeStart || booking.TimeEnd != bookingUpdateDto.TimeEnd)
            {
                switch (booking.Status)
                {
                    case (sbyte) BookingStatus.Paid:
                        return new ResponseDto(null, "Lượt đặt sân đã được thanh toán. Không thể thay đổi thời gian",
                            false, StatusCodes.Status400BadRequest);
                    case (sbyte) BookingStatus.Completed:
                        return new ResponseDto(null, "Lượt đặt sân đã hoàn thành. Không thể thay đổi thời gian",
                            false, StatusCodes.Status400BadRequest);
                    case (sbyte) BookingStatus.Cancelled:
                        return new ResponseDto(null, "Lượt đặt sân đã hủy. Không thể thay đổi thời gian",
                            false, StatusCodes.Status400BadRequest);
                    case (sbyte) BookingStatus.NoShow:
                        return new ResponseDto(null, "Đã diễn ra. Không thể thay đổi thời gian",
                            false, StatusCodes.Status400BadRequest);
                }
                
                var response = await OverallValidate(bookingUpdateDto.CourtId, bookingUpdateDto.Date, bookingUpdateDto.TimeStart,
                    bookingUpdateDto.TimeEnd, bookingUpdateDto.MatchId, bookingUpdateDto.UserId, id);

                if (response.IsSucceed == false) return response;
                
                booking.Cost = (double) response.Result!;
                booking.Status = (sbyte) BookingStatus.Pending;
            }

            if (string.IsNullOrWhiteSpace(bookingUpdateDto.Note))
            {
                booking.Note = bookingUpdateDto.Note;
            }
            
            booking.CreateAt = DateTime.Now;
            
            await unitOfWork.BookingRepo.UpdateAsync(booking);

            var bookingDto = mapper.Map<BookingViewDto>(booking);
            return new ResponseDto(bookingDto, "Cập nhật thành công", true, StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseDto> DeleteBooking(int id)
    {
        try
        {
            var booking = await unitOfWork.BookingRepo.GetByConditionAsync(b => b.BookingId == id, b => b);
            if (booking == null)
                return new ResponseDto(null, "Không có lượt đặt nào với id này", false, StatusCodes.Status404NotFound);
            
            booking.Status = (sbyte) BookingStatus.Cancelled;
            
            await unitOfWork.BookingRepo.DeleteAsync(booking); 
            
            return new ResponseDto(null, "Xóa thành công", true, StatusCodes.Status204NoContent);
        }
        catch (Exception e)
        {
            return new ResponseDto(null, e.Message, false, StatusCodes.Status500InternalServerError);
        }
    }

    private async Task<bool> IsOverLap(int courtId, int? id, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd )
    {
        if (id.HasValue)
        {
            return await unitOfWork.BookingRepo
                .AnyAsync(b => b.CourtId == courtId &&
                               b.BookingId !=  id &&
                               b.Date.Equals(date) &&
                               b.TimeStart < timeEnd &&
                               b.TimeEnd > timeStart);
        }
        
        return await unitOfWork.BookingRepo
            .AnyAsync(b => b.CourtId == courtId &&
                           b.Date.Equals(date) &&
                           b.TimeStart < timeEnd &&
                           b.TimeEnd > timeStart);
    }

    private async Task<ResponseDto> OverallValidate(int courtId, DateOnly date, TimeOnly timeStart, TimeOnly timeEnd, int? matchId, int? userId, int? bookingId)
    {
        var today = DateOnly.FromDateTime(DateTime.Now.Date);

        var timeNow = TimeOnly.FromDateTime(DateTime.Now);
        
        // validate user and match
        if (matchId.HasValue && userId.HasValue)
            return new ResponseDto(null, "Trận đấu và người chơi không thể cùng tạo lượt đặt", false, StatusCodes.Status400BadRequest);
        
        
        // validate match
        if (matchId.HasValue)
        {
            var match = await unitOfWork.MatchRepo.GetByConditionAsync(m => m.MatchId == matchId.Value, 
                m => new {m.MatchId, m.MatchName}, null);
            
            if (match == null)
                return new ResponseDto(null, "Không có trận đấu nào với id này", false, StatusCodes.Status404NotFound);
        }
        
        // validate user
        if (userId.HasValue)
        {
            var user = await unitOfWork.UserRepo.GetByConditionAsync(u => u.UserId == userId.Value, u => new {u.UserId});
            if (user == null)
                return new ResponseDto(null, "Không có người chơi nào với id này", false, StatusCodes.Status404NotFound);
        }
        
        // validate court
        var court = await unitOfWork.CourtRepo.GetCourtById(courtId);
        
        if (court == null)
            return new ResponseDto(null, "Không có sân nào với id này", false, StatusCodes.Status404NotFound);

        // validate date and time start
        switch (date.CompareTo(today))
        {
            case < 0:
                return new ResponseDto(null, "Ngày đặt không thể ở quá khứ", false, StatusCodes.Status400BadRequest);
            case 0:
                if (timeStart.CompareTo(timeNow.AddHours(0.5)) <= 0)
                    return new ResponseDto(null, "Giờ đặt quá sát", false, StatusCodes.Status400BadRequest);
                break;
        }
        
        // validate overlap time
        var isOverLap = await IsOverLap(courtId, bookingId, date, timeStart, timeEnd);

        if (isOverLap)
            return new ResponseDto(null, "Khoảng thời gian đặt trùng với lượt đặt đã tồn tại", false, StatusCodes.Status400BadRequest);
        
        // Validate time range (timeStart < timeEnd)
        if (timeStart >= timeEnd)
            return new ResponseDto(null, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc", false, StatusCodes.Status400BadRequest);
        
        
        // fetch the court slots for the given court
        var courtSlots = await unitOfWork.SlotRepo.FindByConditionAsync(s => s.CourtId == courtId, 
            s => new MatchSlot(
                s.TimeStart,
                s.TimeEnd,
                s.Cost
                ));

        if (courtSlots.Count == 0)
            return new ResponseDto(null, "Sân này không có khung giờ nào sẵn", true, StatusCodes.Status400BadRequest);

        // find slots that match or overlap with the requested timeStart and timeEnd
        var matchingSlots = courtSlots
            .Where(s => s.TimeStart <= timeEnd && s.TimeEnd >= timeStart)
            .OrderBy(s => s.TimeStart)
            .ToList();

        // check if the time range is fully covered
        if (matchingSlots.Count == 0 || matchingSlots.First().TimeStart > timeStart || matchingSlots.Last().TimeEnd < timeEnd)
            return new ResponseDto(null, "Khoảng thời gian đặt không nằm trong các khung giờ có sẵn.", true, StatusCodes.Status400BadRequest);

        // ensure there are no gaps between the slots
        for (var i = 0; i < matchingSlots.Count - 1; i++)
        {
            if (matchingSlots[i].TimeEnd == matchingSlots[i + 1].TimeStart) continue;
            return new ResponseDto(null, "Khoảng thời gian đặt ở hai khung giờ không liền kề", true, StatusCodes.Status400BadRequest);
        }
        
        var totalCost = (from slot in matchingSlots 
                let overlapStart = timeStart > slot.TimeStart ? timeStart : slot.TimeStart 
                let overlapEnd = timeEnd < slot.TimeEnd ? timeEnd : slot.TimeEnd 
                let overlapDuration = (overlapEnd.ToTimeSpan() - overlapStart.ToTimeSpan()).TotalHours 
                select overlapDuration * slot.Cost)
            .Sum();

        var cost = Math.Round(totalCost, 2);
        
        return new ResponseDto(cost, "Thỏa mãn", true, StatusCodes.Status200OK);
    }
}