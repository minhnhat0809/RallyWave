using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Entity;
using Google.Apis.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.BusinessObjects.UserViewModel;
using Microsoft.IdentityModel.Tokens;
using UserManagement.DTOs.UserDto;
using UserManagement.Repository;

namespace Identity.API.Services;

public interface IAuthService
{
    //private string? GenerateJwtToken(User user, string role);
    //public Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token);
    
    
    // Including Role, else return role not exist
    public Task<ResponseGoogleLoginModel> Authenticate(GoogleLoginModel googleLoginDto);

}
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ResponseGoogleLoginModel _responseGoogleLoginModel;
    public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _responseGoogleLoginModel = new ResponseGoogleLoginModel(false, null, null, null);
    }
    
    public async Task<ResponseGoogleLoginModel> Authenticate(GoogleLoginModel request)
    {
        if (request.IdToken != null)
        {
            var payload = await VerifyGoogleToken(request.IdToken);
            
            // User Role Problem:
            //var userList = await _unitOfWork.UserRepo.GetUsers("email", payload.Email) ;
            var userExist = await _unitOfWork.UserRepo.GetUserByEmail(payload.Email);
            // Not Exist -> add new one
            if (userExist == null) 
            {
                User user = new User()
                {
                    UserName = payload.Name,
                    Email = payload.Email,
                    PhoneNumber = 0, // Placeholder
                    Gender = "N/A",
                    Dob = new DateOnly(2000, 1, 1),
                    Address = "N/A",
                    Province = "N/A",
                    Avatar = payload.Picture,
                    Status = 1,
                };
                var newUser = await _unitOfWork.UserRepo.CreateUser(user);

                // Only On Create User
                if (request.Role != null)
                {
                    var token = GenerateJwtToken(user, request.Role);
                    _responseGoogleLoginModel.Message = "Login successfully";
                    _responseGoogleLoginModel.IsSuccess = true;
                    _responseGoogleLoginModel.AccessToken = token;
                    _responseGoogleLoginModel.User = newUser;
                }
            }
            else // Existed - Go get the Token
            {
                var token = GenerateJwtToken(userExist, "User");
                _responseGoogleLoginModel.Message = "Login successfully";
                _responseGoogleLoginModel.IsSuccess = true;
                _responseGoogleLoginModel.AccessToken = token;
                _responseGoogleLoginModel.User = _mapper.Map<UserViewDto>(userExist);
            }
            
            return _responseGoogleLoginModel;
        }

        return _responseGoogleLoginModel;
    }
    
    
    // generate JWT Token 
    private string? GenerateJwtToken(User user, string role)
    {
        if (user.Email != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role) // Role come in Token
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:Jwt:SecretKey"] ?? string.Empty));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        return null;
    }
    // Verify Google Token 
    private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
    {
        try
        {
            // ValidationSettings define the accepted audience (ClientId)
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string?> { _configuration["Authentication:Google:ClientId"] }
            };

            // Validate the token using Google's library
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
            return payload;
        }
        catch (InvalidJwtException ex)
        {
            // Token is invalid or expired
            throw new Exception(ex.Message);
        }
    }

    
}