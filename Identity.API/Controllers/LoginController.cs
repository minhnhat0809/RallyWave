using System.Security.Claims;
using Entity;
using FirebaseAdmin.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
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
        [HttpPost("google")]
        public async Task<ActionResult<ResponseDto>> GoogleResponse([FromBody] LoginModel request)
        {
            
            var responseDto = new ResponseDto(null, null, false, StatusCodes.Status400BadRequest);
            var result = await _authService.Authenticate(request);
            
            responseDto.Message = result.Message;
            responseDto.StatusCode = StatusCodes.Status202Accepted;
            responseDto.IsSucceed = result.IsSuccess;
            responseDto.Result = result;
            
            if (!result.IsSuccess)
            {
                return BadRequest(responseDto);
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
