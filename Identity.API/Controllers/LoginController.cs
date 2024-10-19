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
        ResponseModel _responseModel;
        
        public LoginController(IAuthService authService)
        {
            _authService = authService;
            _responseModel = new ResponseModel(null, null, false, StatusCodes.Status400BadRequest);
        }
        
        [HttpPost("google")]
        public async Task<ActionResult<ResponseModel>> LoginByGoogle(RequestGoogleLoginModel request)
        {
            try
            {
                var result = await _authService.LoginByGoogle(request);
                _responseModel = new ResponseModel(result, "Login by Google successful", true, StatusCodes.Status200OK);
                return Ok(_responseModel);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel>> LoginByGoogle(RequestRegisterModel request)
        {
            try
            {
                var result = await _authService.Register(request);
                _responseModel = new ResponseModel(result, "Login by Google successful", true, StatusCodes.Status200OK);
                return Ok(_responseModel);
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
                _responseModel = new ResponseModel(result, "Login successful", true, StatusCodes.Status200OK);
                return Ok(_responseModel);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("verify-email")]
        public async Task<ActionResult<ResponseModel>> VerifyByEmail(RequestVerifyModel request)
        {
            try
            {
                var result = await _authService.VerifyEmail(request);
                _responseModel = new ResponseModel(result, "Verify successful", true, StatusCodes.Status200OK);
                return Ok(_responseModel);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
    }

}
