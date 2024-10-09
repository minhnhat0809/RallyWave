using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity;
using Google.Apis.Auth;
using Identity.API.BusinessObjects.RequestObject;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public AuthController(IConfiguration configuration, IUserService userService, IAuthService authService)
    {
        _configuration = configuration;
        _userService = userService;
        _authService = authService;
    }

    // Step 1: Redirect user to Google for authentication
    [HttpGet("google-login")]
public IActionResult GoogleLogin()
{
    var redirectUrl = Url.Action("GoogleResponse", "Auth");
    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
    
    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
}


    // Step 2: Handle the response from Google after authentication
    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
{
    // Authenticate the user
    var info = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    
    if (info?.Principal == null)
    {
        return BadRequest("Unable to authenticate with Google.");
    }

    // Extract user information from claims
    var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
    var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value;
    var picture = info.Principal.FindFirst("picture")?.Value; // Optional

    // Check if the user exists in the database
    var user = await _userService.GetUserByEmailAsync(email);
    if (user == null)
    {
        // If user doesn't exist, create a new user
        user = new User
        {
            UserName = name,
            Email = email,
            PhoneNumber = 0, // Placeholder
            Gender = "Unknown", // Placeholder
            Dob = new DateOnly(2000, 1, 1), // Default DOB
            Address = "Not Provided", // Placeholder
            Province = "Not Provided", // Placeholder
            Avatar = picture,
            Status = 1, 
        };
        await _userService.CreateUserAsync(user);
    }

    // Return user information without generating a JWT
    return Ok(new
    {
        Message = "Successfully authenticated with Google.",
        Email = email,
        Name = name,
        Picture = picture // Optional: return user picture
    });
}



    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
    {
        try
        {
            // ValidationSettings define the accepted audience (ClientId)
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string?> { _configuration["Google:ClientId"] }
            };

            // Validate the token using Google's library
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
        catch (InvalidJwtException)
        {
            // Token is invalid or expired
            return null;
        }
    }
}
