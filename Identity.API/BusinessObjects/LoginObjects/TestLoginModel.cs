namespace Identity.API.BusinessObjects.LoginObjects;

public class TestGoogleLoginModel(bool isSuccess, string? message, string? accessToken, string? idToken)
{
    public bool IsSuccess { get; set; } = isSuccess;
    public string? Message { get; set; } = message;
    public string? AccessToken { get; set; } = accessToken;
    public string? IdToken { get; set; } = idToken;
}