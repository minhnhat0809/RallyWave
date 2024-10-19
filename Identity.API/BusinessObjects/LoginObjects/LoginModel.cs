using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.BusinessObjects.LoginObjects;
// Login Model
public class RequestLoginModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
