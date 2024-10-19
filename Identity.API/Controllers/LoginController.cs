using System.Security.Claims;
using Entity;
using FirebaseAdmin.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
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
        private readonly ResponseModel _responseModel;
        
        public LoginController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
            _responseModel = new ResponseModel(null, null, false, StatusCodes.Status400BadRequest);
        }
        
        [HttpPost("google")]
        public async Task<ActionResult<ResponseModel>> GoogleResponse([FromBody] GoogleLoginModel request)
        {
            
            var result = await _authService.Login(request);
            
            _responseModel.Message = result.Message;
            _responseModel.StatusCode = StatusCodes.Status202Accepted;
            _responseModel.IsSucceed = result.IsSuccess;
            _responseModel.Result = result;
            
            if (!result.IsSuccess)
            {
                return BadRequest(_responseModel);
            }

            return Ok(_responseModel);
        }
        
        

        [HttpPost("phone")]
        public async Task<ActionResult<ResponseModel>> LoginWithPhone([FromBody] PhoneLoginRequest request)
        {
            var result = await _authService.SendPhoneVerificationAsync(request);
            if (result.IsSucceed == false)
            {
                return BadRequest(_responseModel.Result = result);
            }

            return Ok(new ResponseModel(result, "Verification code sent", true, StatusCodes.Status200OK));
        }
        

        [HttpPost("sms-verify-code")]
        public async Task<ActionResult<ResponseModel>> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var result = await _authService.VerifyPhoneCodeAsync(request);
            if (result.IsSucceed == false)
            {
                return BadRequest(_responseModel.Result = result);
            }
            return Ok(new ResponseModel("Login successful", "", false, StatusCodes.Status200OK));
        }
        
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out");
        }
    }

}
