using System.Text;
using Entity;

namespace WebApplication1.Repository.Impl;

public class BookingRepo(RallywaveContext repositoryContext) : RepositoryBase<Booking>(repositoryContext), IBookingRepo
{
    public async Task<List<Booking>> GetBookings(string? filterField, string? filterValue)
    {
        var bookings = new List<Booking>();
        try
        {
            if (string.IsNullOrEmpty(filterField) || string.IsNullOrEmpty(filterValue))
            {
                bookings = await FindAllAsync(b => b.Court, b => b.Match, b => b.User, b => b.PaymentDetails);
            }
            else
            {
                switch (filterField.ToLower())
                {
                    case "date":
                        var date = DateOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.Date.Equals(date), b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetails);
                        break;
                    
                    case "timestart":
                        var timeStart = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeStart),b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetails);
                        break;
                    
                    case "timeend":
                        var timeEnd = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(timeEnd),b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetails);
                        break;
                    
                    case "createat":
                        var createAt = TimeOnly.Parse(filterValue);
                        bookings = await FindByConditionAsync(b => b.TimeStart.Equals(createAt),b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetails);
                        break;
                    case "status":
                        var status = Encoding.UTF8.GetBytes(filterValue);
                        bookings = await FindByConditionAsync(b => b.Status.Equals(status),b => b.Court,
                            b => b.Match, b => b.User, b => b.PaymentDetails);
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
}