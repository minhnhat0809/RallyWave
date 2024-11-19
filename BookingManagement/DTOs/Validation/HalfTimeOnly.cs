using System.ComponentModel.DataAnnotations;

namespace BookingManagement.DTOs.Validation;

public class HalfHourOnlyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is TimeOnly time)
        {
            return time.Minute is 0 or 30 ? ValidationResult.Success : new ValidationResult("Time must be at the start of the hour or half-hour (minutes should be 00 or 30).");
        }
        return new ValidationResult("Invalid time format.");
    }
}