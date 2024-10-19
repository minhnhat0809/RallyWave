using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.BusinessObjects.LoginObjects;

public class RequestGoogleLoginModel
{
    public required string IdToken { get; set; }
    public string? Role { get; set; }
}

public class ResponseLoginModel(string? accessToken, string? firebaseToken, UserViewDto? user, bool isNewUser)
{
    public string? AccessToken { get; set; } = accessToken;
    public string? FirebaseToken { get; set; } = firebaseToken;
    public UserViewDto? User { get; set; } = user;
    public bool IsNewUser { get; set; } = isNewUser; // Indicates if the user was just created
}