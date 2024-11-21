using System.ComponentModel.DataAnnotations;

namespace BookingManagement.DTOs.Validation;

public class MinDate: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateOnly date) return ValidationResult.Success;
        
        return date < DateOnly.FromDateTime(DateTime.Today) ? new ValidationResult("The date cannot be in the past.") : ValidationResult.Success;
    }
}

public class EndTime(string startTimePropertyName) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var endTime = value as TimeOnly?;
        var startTimeProperty = validationContext.ObjectType.GetProperty(startTimePropertyName);
            
        if (startTimeProperty == null)
            return new ValidationResult($"Unknown property: {startTimePropertyName}");

        var startTime = (TimeOnly?)startTimeProperty.GetValue(validationContext.ObjectInstance);

        return endTime <= startTime ? new ValidationResult("End time must be later than start time.") : ValidationResult.Success;
    }
}