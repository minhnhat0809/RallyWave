namespace Identity.API.BusinessObjects.LoginObjects;

public class GoogleLoginModel
{
    public string? IdToken { get; set; }
    public string? Role { get; set; }
}

public class ResponseGoogleLoginModel
{
    public bool IsSuccess { get; set; }
    public Object? User { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }

    public ResponseGoogleLoginModel(bool isSuccess, object? user, string? message, string? accessToken)
    {
        IsSuccess = isSuccess;
        User = user;
        Message = message;
        AccessToken = accessToken;
    }
}