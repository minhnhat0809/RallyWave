namespace Identity.API.BusinessObjects.LoginObjects;
public class RequestVerifyModel
{
    public required string Email { get; set; }
    public required string VerificationCode { get; set; } // Code sent to the user's email
    public required string NewPassword { get; set; } // Optional for resetting password
}
public class ResponseVerifyModel(bool isVerified, string? message)
{
    public bool IsVerified { get; set; } = isVerified;
    public string? Message { get; set; } = message;
}
