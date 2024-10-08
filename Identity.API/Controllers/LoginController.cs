using System.Security.Claims;
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
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            if (info?.Principal == null)
            {
                return BadRequest("Unable to authenticate with Google.");
            }

            // Extract user information from the claims
            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value;

            // Here, you can create or update the user in your database
            // await _userService.CreateOrUpdateUserAsync(email, name);

            return Ok(new
            {
                Message = "Successfully authenticated with Google.",
                Email = email,
                Name = name
            });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }
}
