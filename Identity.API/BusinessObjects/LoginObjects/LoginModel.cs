namespace Identity.API.BusinessObjects.LoginObjects;

public class LoginModel
{
    public string? IdToken { get; set; }
    public string? Role { get; set; }
}

public class ResponseLoginModel
{
    public bool IsSuccess { get; set; }
    public Object? User { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }

    public ResponseLoginModel(bool isSuccess, object? user, string? message, string? accessToken)
    {
        IsSuccess = isSuccess;
        User = user;
        Message = message;
        AccessToken = accessToken;
    }
}