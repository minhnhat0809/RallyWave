namespace BookingManagement.Enum;

public enum BookingStatus : sbyte
{
    Pending = 0,
    Confirmed = 1,
    Paid = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}