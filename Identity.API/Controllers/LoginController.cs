using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("login-google")]
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
        
        [HttpPost("forget-password")]
        public async Task<ActionResult<ResponseModel>> ForgetPassword(string email)
        {
            try
            {
                var result = await _authService.ForgetPassword(email);
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
        [HttpPost("resend-verify-code")]
        public async Task<ActionResult<ResponseModel>> ResendVerificationEmailAccount([FromQuery] string email)
        {
            try
            {
                var result = await _authService.ResendVerifyCode(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("verify-password")]
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
        
        [HttpPut("profile")]
        public async Task<ActionResult<ResponseModel>> UpdateProfile(ProfileModel request)
        {
            try
            {
                var result = await _authService.UpdateProfile(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("upload-avatar")]
        public async Task<ActionResult<ResponseModel>> UploadAvatar( IFormFile avatar, string email)
        {
            try
            {
                var response = await _authService.UploadUserAvatarAsync(avatar, email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("delete-avatar")]
        public async Task<ActionResult<ResponseModel>> DeleteAvatar([FromQuery] string email)
        {
            try
            {
                var response = await _authService.DeleteUserAvatarAsync(email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return new ResponseModel(null, ex.Message, false, StatusCodes.Status500InternalServerError);
            }
        }
        
    }

}
