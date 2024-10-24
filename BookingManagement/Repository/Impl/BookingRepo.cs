using System.Linq.Expressions;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement.Repository.Impl;

public class BookingRepo(RallyWaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    public async Task<ResponseListDto<BookingsViewDto>> GetBookings(string? subject, int? subjectId, string filterField, string filterValue, int pageNumber, int pageSize)
    {
        try
        {
            Expression<Func<Booking, bool>> basePredicate = b => true;
            
            if (!string.IsNullOrWhiteSpace(subject)  && subjectId != null)
            {
                basePredicate = subject.ToLower() switch
                {
                    "user" => b => b.UserId.HasValue && b.UserId.Value == subjectId.Value,
                    "court" => b => b.CourtId == subjectId.Value,
                    _ => throw new ArgumentException($"Unknown subject '{subject}'")
                };
            }
            
            switch (filterField.ToLower())
            {
                case "date":
                    var date = DateOnly.Parse(filterValue);
                    var dateCondition = Expression.Equal(
                        Expression.Property(basePredicate.Parameters[0], nameof(Booking.Date)),
                        Expression.Constant(date)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, dateCondition),
                        basePredicate.Parameters
                    );
                    break;

                case "timestart":
                    if (TimeOnly.TryParse(filterValue, out var timeStart))
                    {
                        var timeStartCondition = Expression.Equal(
                            Expression.Property(basePredicate.Parameters[0], nameof(Booking.TimeStart)),
                            Expression.Constant(timeStart)
                        );
                        basePredicate = Expression.Lambda<Func<Booking, bool>>(
                            Expression.AndAlso(basePredicate.Body, timeStartCondition),
                            basePredicate.Parameters
                        );
                    }
                    break;

                case "timeend":
                    var timeEnd = TimeOnly.Parse(filterValue);
                    var timeEndCondition = Expression.Equal(
                        Expression.Property(basePredicate.Parameters[0], nameof(Booking.TimeEnd)),
                        Expression.Constant(timeEnd)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, timeEndCondition),
                        basePredicate.Parameters
                    );
                    break;

                case "createat":
                    if (DateTime.TryParse(filterValue, out var createdAt))
                    {
                        var createdAtCondition = Expression.Equal(
                            Expression.Property(basePredicate.Parameters[0], nameof(Booking.CreateAt)),
                            Expression.Constant(createdAt)
                        );
                        basePredicate = Expression.Lambda<Func<Booking, bool>>(
                            Expression.AndAlso(basePredicate.Body, createdAtCondition),
                            basePredicate.Parameters
                        );
                    }
                    break;

                case "status":
                    if (sbyte.TryParse(filterValue, out var status))
                    {
                        var statusCondition = Expression.Equal(
                            Expression.Property(basePredicate.Parameters[0], nameof(Booking.Status)),
                            Expression.Constant(status)
                        );
                        basePredicate = Expression.Lambda<Func<Booking, bool>>(
                            Expression.AndAlso(basePredicate.Body, statusCondition),
                            basePredicate.Parameters
                        );
                    }
                    break;
            }

            var total = await CountByConditionAsync(basePredicate);
            
            var bookings = await FindByConditionWithPagingAsync(
                basePredicate,
                b => new BookingsViewDto(b.BookingId, b.Date, b.TimeStart, b.TimeEnd, b.Cost, b.Status),
                pageSize, pageSize);

            var responseDto = new ResponseListDto<BookingsViewDto>(bookings, total);

            return responseDto;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<Booking?> GetBookingById(int bookingId)
    {
        try
        {
            return await GetByConditionAsync(b => b.BookingId == bookingId, b => b,  b => b.Court!,
                b => b.Match, b => b.User, b => b.PaymentDetail);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task CreateBooking(Booking booking)
    {
        try
        {
            await CreateAsync(booking);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task UpdateBooking(Booking booking)
    {
        try
        {
            await UpdateAsync(booking);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task DeleteBooking(Booking booking)
    {
        try
        {
            await DeleteAsync(booking);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
}