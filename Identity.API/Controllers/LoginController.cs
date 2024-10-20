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
        private readonly IAuthService _authService;
        
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("google")]
        public async Task<ActionResult<ResponseModel>> LoginByGoogle(RequestGoogleLoginModel request)
        {
            try
            {
                var result = await _authService.LoginByGoogle(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel>> Register(RequestRegisterModel request)
        {
            try
            {
                var result = await _authService.Register(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel>> Login(RequestLoginModel request)
        {
            try
            {
                var result = await _authService.Login(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("reset-password")]
        public async Task<ActionResult<ResponseModel>> ResetPassword(RequestLoginModel request)
        {
            try
            {
                var result = await _authService.ResetPassword(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("verify-account")]
        public async Task<ActionResult<ResponseModel>> VerifyAccount(RequestVerifyAccountModel request)
        {
            try
            {
                var result = await _authService.VerifyEmailAccount(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("verify-password-reset")]
        public async Task<ActionResult<ResponseModel>> VerifyPasswordReset(RequestVerifyModel request)
        {
            try
            {
                var result = await _authService.VerifyEmailResetPassword(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
    }

}
