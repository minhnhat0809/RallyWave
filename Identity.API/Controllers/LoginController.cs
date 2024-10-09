using System.Security.Claims;
using Entity;
using Identity.API.BusinessObjects;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService; 
        public LoginController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }
        // Action to redirect user to Google for authentication
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Login"); // Define the redirect action after Google login
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            
            return Challenge(properties, GoogleDefaults.AuthenticationScheme); // Challenge Google authentication
        }

        // Action to handle the response from Google after authentication
        [HttpGet("google-response")]
        [Authorize]
        public async Task<ActionResult<ResponseModel>?> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var responseModel = new ResponseModel();

            if (result?.Principal == null)
            {
                responseModel.IsSuccessful = false;
                responseModel.Error = "Unable to authenticate with Google.";
                return BadRequest(responseModel);
            }

            // Lấy access token và id token
            responseModel.AccessToken = result.Properties?.GetTokenValue("access_token");
            responseModel.IdToken = result.Properties?.GetTokenValue("id_token");

            if (string.IsNullOrEmpty(responseModel.IdToken))
            {
                responseModel.IsSuccessful = false;
                responseModel.Error = "ID Token is missing.";
                return BadRequest(responseModel);
            }
            return Ok(responseModel);
        }

        [HttpGet("google-response-uncensored")]
        public async Task<IActionResult> GoogleResponse([FromQuery] string idToken)
        {
            var payload = await _authService.VerifyGoogleToken(idToken);
            var responseModel = new ResponseModel();
            if (payload == null)
            {
                return BadRequest("Invalid Google token.");
            }

            // Use payload information to authenticate or create a user
            var email = payload.Email;
            var name = payload.Name;
            var picture = payload.Picture; // Optional

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = name,
                    Email = email,
                    PhoneNumber = 0, // Placeholder
                    Gender = "N/A",
                    Dob = new DateOnly(2000, 1, 1) ,
                    Address =  "N/A",
                    Province =  "N/A",
                    Avatar = picture,
                    Status = 1,
                    
                };
                responseModel.User = await _userService.AddUserAsync(user);
            }


            return Ok(responseModel);
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }
}
