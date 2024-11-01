using Identity.API.BusinessObjects.UserViewModel;

namespace Identity.API.BusinessObjects.LoginObjects;
// Login Model
public class RequestLoginModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
