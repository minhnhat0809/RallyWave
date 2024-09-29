using System.Text;
using BookingManagement.DTOs.BookingDto;
using Entity;

namespace BookingManagement.Repository.Impl;

public class BookingRepo(RallywaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    public async Task<List<Booking>> GetBookings(string? filterField, string? filterValue)
    {
        var bookings = new List<Booking>();
        try
        {
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                bookings = await FindAllAsync(b => b.Court ?? new Court(),
                    b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                
            }
            else
            {
                switch (filterField.ToLower())
                {
                    case "date":
                        var date = DateOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.Date.Equals(date), b => b.Court ?? new Court(),
                            b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                        break;
                    
                    case "timestart":
                        var timeStart = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeStart),b => b.Court ?? new Court(),
                            b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                        break;
                    
                    case "timeend":
                        var timeEnd = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeEnd),b => b.Court ?? new Court(),
                            b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                        break;
                    
                    case "createat":
                        var createAt = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(createAt),b => b.Court ?? new Court(),
                            b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                        break;
                    case "status":
                        var status = Encoding.UTF8.GetBytes(filterValue);
                        bookings = await FindByConditionAsync(b => b.Status.Equals(status),b => b.Court ?? new Court(),
                            b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
                        break;
                }
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
            return await GetByIdAsync(bookingId, b => b.Court ?? new Court(),
                b => b.Match ?? new Match(), b => b.User ?? new User(), b => b.PaymentDetails);
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