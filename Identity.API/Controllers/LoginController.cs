using System.Security.Claims;
using Entity;
using FirebaseAdmin.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.RequestObject;
using Identity.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs.UserDto;

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
        public async Task<ActionResult<ResponseDto>> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var responseDto = new ResponseDto(null, null, false, StatusCodes.Status400BadRequest);

            if (result?.Principal == null)
            {
                responseDto.Message = "Unable to authenticate with Google.";
                return BadRequest(responseDto);
            }

            // Get access token and id token
            var accessToken = result.Properties?.GetTokenValue("access_token");
            var idToken = result.Properties?.GetTokenValue("id_token");

            if (string.IsNullOrEmpty(idToken))
            {
                responseDto.Message = "ID Token is missing.";
                return BadRequest(responseDto);
            }

            responseDto.Result = new { AccessToken = accessToken, IdToken = idToken };
            responseDto.IsSucceed = true;
            responseDto.StatusCode = StatusCodes.Status200OK;

            return Ok(responseDto);
        }

        [HttpPost("google-response-uncensored")]
        public async Task<ActionResult<ResponseDto>> GoogleResponse([FromBody] GoogleLoginRequest request)
        {
            var payload = await _authService.VerifyGoogleToken(request.Token);
            var responseDto = new ResponseDto(null, null, false, StatusCodes.Status400BadRequest);
            if (payload == null)
            {
                responseDto.Message = "Invalid Google token.";
                return BadRequest(responseDto);
            }

            // Use payload information to authenticate or create a user
            var email = payload.Email;
            var name = payload.Name;
            var picture = payload.Picture; // Optional

            var emailExist = await _userService.GetUserByEmailAsync(email);
            if (emailExist.Result == null)
            {
                UserCreateDto user = new UserCreateDto()
                {
                    UserName = name,
                    Email = email,
                    PhoneNumber = 0, // Placeholder
                    Gender = "N/A",
                    Dob = new DateOnly(2000, 1, 1),
                    Address = "N/A",
                    Province = "N/A",
                    Avatar = picture,
                    Status = 1,
                };
                responseDto = await _userService.CreateUser(user);
            }
            
            return Ok(responseDto);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }

}
