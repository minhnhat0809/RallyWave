using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Entity;
using Google.Apis.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.BusinessObjects.UserViewModel;
using Identity.API.Repository;
using Microsoft.IdentityModel.Tokens;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UserManagement.DTOs.UserDto;

namespace Identity.API.Services;

public interface IAuthService
{
    public Task<ResponseGoogleLoginModel> Login(GoogleLoginModel googleLoginDto);
}


public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ResponseGoogleLoginModel _responseGoogleLoginModel;
    private readonly ResponseModel _responseModel;
    public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _responseGoogleLoginModel = new ResponseGoogleLoginModel(false, null, null, null);
        _responseModel = new ResponseModel(null, null, false, StatusCodes.Status400BadRequest);
    }
    // login by GOOGLE
    public async Task<ResponseGoogleLoginModel> Login(GoogleLoginModel request)
    {
        try
        {
            // Verify the Google ID token with Firebase
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string firebaseUid = decodedToken.Uid;

            // Check if user exists in the database
            var user = _context.Users.FirstOrDefault(u => u.FirebaseUid == firebaseUid);
            if (user == null)
            {
                // Register the user if not found
                var userInfo = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseUid);
                    
                user = new User
                {
                    UserName = userInfo.DisplayName,
                    Email = userInfo.Email,
                    FirebaseUid = firebaseUid,
                    PhoneNumber = userInfo.PhoneNumber != null ? Convert.ToInt32(userInfo.PhoneNumber) : 0,
                    Gender = "Not specified",  // Set default value or change as per your needs
                    Dob = DateOnly.FromDateTime(DateTime.Now),  // Set default DOB or use actual value
                    Address = "Unknown",
                    Province = "Unknown",
                    Status = 1,
                    Avatar = userInfo.PhotoUrl
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // User exists or was registered successfully - issue JWT or other session management token
            // Return success message (and token if using JWT)
            return Ok(new { message = "Login successful", userId = user.UserId });
        }
        catch (Exception ex)
        {
            return _responseGoogleLoginModel { message = "Invalid token", error = ex.Message });
        }
    }
    // login by PHONE (send sms verify)
    public async Task<ResponseModel> SendPhoneVerificationAsync(PhoneLoginRequest request)
    {
        var accountSid = _configuration["Authentication:Twilio:AccountSid"];
        var authToken = _configuration["Authentication:Twilio:AuthToken"];
        var serviceSid = _configuration["Authentication:Twilio:ServiceSid"];

        // Initialize the Twilio client with AccountSid and AuthToken
        TwilioClient.Init(accountSid, authToken);
        var phone = "+84" + int.Parse(request.PhoneNumber);
        var verification = await VerificationResource.CreateAsync(
            to: "+840399448325",
            channel: "sms",
            pathServiceSid: serviceSid
        );

        if (verification.Status == "pending")
        {
            _responseModel.IsSucceed = true;
            _responseModel.Message = "Verification code sent successfully.";
            return _responseModel;
        }
        _responseModel.IsSucceed = false;
        _responseModel.Message ="Failed to send verification code." ;
        return _responseModel;
    }   
    // verify SMS CODE to response Token
    public async Task<ResponseModel> VerifyPhoneCodeAsync(VerifyCodeRequest phoneVerificationModel)
    {
        var serviceSid = _configuration["Authentication:Twilio:ServiceSid"];
        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: "+84" + phoneVerificationModel.PhoneNumber,
            code: phoneVerificationModel.EnteredCode,
            pathServiceSid: serviceSid
        );

        if (verificationCheck.Status == "approved")
        {
            // Handle login, token generation, or user creation if needed
            if (phoneVerificationModel.PhoneNumber != null)
            {
                User? user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("phoneNumber",phoneVerificationModel.PhoneNumber);

                if (user == null)
                {
                    // Create a new user if necessary - need to claim User's Name, Email, Role, Phone
                    User newUser = new User()
                    {
                        PhoneNumber = int.Parse(phoneVerificationModel.PhoneNumber),
                        UserName = "NewUser", // Placeholder
                        Email = "user@example.com" // Placeholder
                    };
                    await _unitOfWork.UserRepo.CreateUser(newUser);
                    user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("phoneNumber", phoneVerificationModel.PhoneNumber);
                }

                // Generate a JWT token for the user
                var token = GenerateJwtToken(user, "User");
                
                _responseModel.IsSucceed = true;
                _responseModel.Result = token;
                _responseModel.Message = "Phone verification successful.";
                return _responseModel;
            }
        }
        _responseModel.IsSucceed = false;
        _responseModel.Message = "Invalid verification code.";
        return _responseModel;
    }

    
    
    

    
}