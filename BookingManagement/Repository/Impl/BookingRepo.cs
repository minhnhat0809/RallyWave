using System.Linq.Expressions;
using BookingManagement.DTOs;
using BookingManagement.DTOs.BookingDto.ViewDto;
using Entity;

namespace BookingManagement.Repository.Impl;

public class BookingRepo(RallyWaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    public async Task<ResponseListDto<BookingsViewDto>> GetBookings(
        string? subject, int? subjectId,
        BookingFilterDto? bookingFilterDto,
        string? sortField, string sortValue,
        int pageNumber, int pageSize)
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
            
            var parameter = basePredicate.Parameters[0]; // Extract the parameter to reuse it in each condition

            if (bookingFilterDto != null)
            {
                // Filter by specific date if provided
                if (bookingFilterDto.Date.HasValue)
                {
                    var dateCondition = Expression.Equal(
                        Expression.Property(parameter, nameof(Booking.Date)),
                        Expression.Constant(bookingFilterDto.Date.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, dateCondition),
                        basePredicate.Parameters
                    );
                }
                else if (bookingFilterDto.DateFrom.HasValue)
                {
                    var dateFromCondition = Expression.GreaterThanOrEqual(
                        Expression.Property(parameter, nameof(Booking.Date)),
                        Expression.Constant(bookingFilterDto.DateFrom.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, dateFromCondition),
                        basePredicate.Parameters
                    );
                }
                else if (bookingFilterDto.DateTo.HasValue)
                {
                    var dateToCondition = Expression.LessThanOrEqual(
                        Expression.Property(parameter, nameof(Booking.Date)),
                        Expression.Constant(bookingFilterDto.DateTo.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, dateToCondition),
                        basePredicate.Parameters
                    );
                }

                // Filter by TimeStart if provided
                if (bookingFilterDto.TimeStart.HasValue)
                {
                    var timeStartCondition = Expression.GreaterThanOrEqual(
                        Expression.Property(parameter, nameof(Booking.TimeStart)),
                        Expression.Constant(bookingFilterDto.TimeStart.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, timeStartCondition),
                        basePredicate.Parameters
                    );
                }

                // Filter by TimeEnd if provided
                if (bookingFilterDto.TimeEnd.HasValue)
                {
                    var timeEndCondition = Expression.LessThanOrEqual(
                        Expression.Property(parameter, nameof(Booking.TimeEnd)),
                        Expression.Constant(bookingFilterDto.TimeEnd.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, timeEndCondition),
                        basePredicate.Parameters
                    );
                }

                // Filter by Status if provided
                if (bookingFilterDto.Status.HasValue)
                {
                    var statusCondition = Expression.Equal(
                        Expression.Property(parameter, nameof(Booking.Status)),
                        Expression.Constant(bookingFilterDto.Status.Value)
                    );
                    basePredicate = Expression.Lambda<Func<Booking, bool>>(
                        Expression.AndAlso(basePredicate.Body, statusCondition),
                        basePredicate.Parameters
                    );
                }
            }
            
            Expression<Func<Booking, object>> orderByExpression = b => b.Date;

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = "timestart";
            }
            
            Expression<Func<Booking, object>> thenOrderByExpression = sortField.ToLower() switch
            {
                "timestart" => b => b.TimeStart,
                "timeend" => b => b.TimeEnd,
                "status" => b => b.Status,
                _ => throw new ArgumentException($"Unknown sorting column '{sortField}'")
            };

            var isAscending = sortValue.ToLower() switch
            {
                "asc" => true,
                "desc" => false,
                _ => true
            };


            var total = await CountByConditionAsync(basePredicate);
            
            var bookings = await FindByConditionWithSortingAndPagingAsync(
                basePredicate,
                b => new BookingsViewDto(b.BookingId, b.Date, b.TimeStart, b.TimeEnd, b.Cost, b.Status),
                pageNumber, pageSize,
                orderByExpression, thenOrderByExpression, isAscending, isAscending
                );

            var responseDto = new ResponseListDto<BookingsViewDto>(bookings, total);

            return responseDto;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}