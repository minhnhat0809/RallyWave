using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.BusinessObjects.LoginObjects;
// Register Model
public class RequestRegisterModel
{
    public required string Username { get; set; }
    
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; } // Optional for additional verification
    public required string Role { get; set; } // Optional for user roles
}
public class ResponseRegisterModel(string? firebaseToken, UserViewDto? user)
{
    public string? FirebaseToken { get; set; } = firebaseToken;
    public UserViewDto? User { get; set; } = user;
}