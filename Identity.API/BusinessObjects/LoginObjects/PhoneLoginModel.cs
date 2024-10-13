namespace Identity.API.BusinessObjects.LoginObjects;

public class PhoneLoginRequest
{
    public string? PhoneNumber { get; set; }
}

public class VerifyCodeRequest
{
    public string? PhoneNumber { get; set; }
    public string? EnteredCode { get; set; }
}