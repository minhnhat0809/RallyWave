﻿using BookingManagement.Enum;
using BookingManagement.Ultility;
using Entity;

namespace BookingManagement.Repository.Impl;

public class BookingRepo(RallywaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    public async Task<List<Booking>> GetBookings(string? filterField, string? filterValue)
    {
        var bookings = new List<Booking>();
        try
        {
            switch (filterField!.ToLower())
                {
                    case "date":
                        var date = DateOnly.Parse(filterValue!);
                        bookings = await FindByConditionAsync(b => b.Date.Equals(date), b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetail);
                        break;
                    
                    case "timestart":
                        var timeStart = TimeOnly.Parse(filterValue!);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeStart), b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetail);
                        break;
                    
                    case "timeend":
                        var timeEnd = TimeOnly.Parse(filterValue!);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeEnd), b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetail);
                        break;
                    
                    case "createat":
                        var createAt = TimeOnly.Parse(filterValue!);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(createAt), b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetail);
                        break;
                    case "status":
                        if (System.Enum.TryParse<BookingStatus>(filterValue, true, out var status))
                        {
                            bookings = await FindByConditionAsync(b => b.Status.Equals(status),
                                b => b.Court,
                                b => b.Match, b => b.User, b => b.PaymentDetail);
                        }
                        break;
                }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        return bookings;
    }

    public async Task<Booking?> GetBookingById(int bookingId)
    {
        try
        {
            return await GetByIdAsync(bookingId,  b => b.Court,
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