using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Entity;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.BusinessObjects.UserViewModel;
using Identity.API.Repository;
using Identity.API.Repository.Impl;
using Microsoft.IdentityModel.Tokens;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UserManagement.DTOs.UserDto;

namespace Identity.API.Services;

public interface IAuthService
{
    
    public Task<ResponseModel> Login(RequestLoginModel request);
    public Task<ResponseModel> LoginByGoogle(RequestGoogleLoginModel request);
    public Task<ResponseModel> Register(RequestRegisterModel request);
    public Task<ResponseModel> VerifyEmail(RequestVerifyModel requestVerifyDto);
    public Task<ResponseLoginModel> ResetPassword(RequestLoginModel request);
}


public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ResponseLoginModel _responseLoginModel;
    private readonly ResponseModel _responseModel;
    public AuthService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _responseLoginModel = new ResponseLoginModel(String.Empty, String.Empty, null, false);
        _responseModel = new ResponseModel(null, String.Empty, false, StatusCodes.Status500InternalServerError);
    }
    // login by Username Password
    public async Task<ResponseModel> Login(RequestLoginModel request)
    {
        try
        {
            // Fetch user from the database
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("username", request.Username);
            if (user != null)
            {
                // Verify the password
                if (user is { PasswordSalt: not null, PasswordHash: not null })
                {
                    var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.Password, user.PasswordSalt);
                    if (!hashedPassword.SequenceEqual(user.PasswordHash))
                    {
                        return new ResponseModel(
                            new ResponseLoginModel(String.Empty, String.Empty, null, false),
                            "Invalid password or username", true, StatusCodes.Status404NotFound);
                    }
                }

                // Generate JWT Token
                var token = _unitOfWork.AuthRepository.GenerateJwtToken(user, "user");
                var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.FirebaseUid);
                
                return new ResponseModel(
                    new ResponseLoginModel(token, firebaseToken, _mapper.Map<UserViewDto>(user), false),
                    "Login Successfully", true, StatusCodes.Status200OK);
            }
            return new ResponseModel(
                null,
                "Invalid Username or Password", true, StatusCodes.Status404NotFound);
        }
        catch (Exception e)
        {
            return new ResponseModel(
                null,
                $"An error occurred during login: {e.Message}", true, StatusCodes.Status500InternalServerError);
        }
    }
    
    // Login By Google && Register
    public async Task<ResponseModel> LoginByGoogle(RequestGoogleLoginModel request)
    {
        try
        {
            // Validate the Google ID Token
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            if (payload == null)
            {
                return new ResponseModel(
                    null,
                    "Invalid Google ID token.", true, StatusCodes.Status404NotFound);
            }

            UserRecord userRecord;
            User? user;
            // Check if user exists in Firebase Authentication
            try
            {
                // Try to get the user by UID (Google UID is used as Firebase UID)
                userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(payload.Subject);
                user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("firebase-uid", userRecord.Uid);
            }
            catch (FirebaseAuthException)
            {
                // If user does not exist, create a new Firebase user
                userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Uid = payload.Subject, // Use Google UID as Firebase UID
                    Email = payload.Email,
                    DisplayName = payload.Name,
                    PhotoUrl = payload.Picture
                });
                
                // Create a new User entity
                var salt = _unitOfWork.AuthRepository.GenerateSalt(); 
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword("Abc@Abc123", salt);
                
                // Create a new user in your application's database
                var newUser = new User
                {
                    UserName = payload.Name,
                    Email = payload.Email,
                    FirebaseUid = payload.Subject, // Store Google UID as well
                    Avatar = payload.Picture,
                    PhoneNumber = 0, 
                    Gender = "N/A",
                    Dob = DateOnly.FromDateTime(DateTime.Now), 
                    Address = "N/A", 
                    Province = "N/A", 
                    Status = 1,
                    PasswordHash = hashedPassword, // set pass default: Abc@Abc1 
                    PasswordSalt = salt,
                    IsTwoFactorEnabled = 1, // 1 is Email verified
                    TwoFactorSecret = null,
                };

                user = await _unitOfWork.UserRepo.CreateUser(newUser); // Save user in the database
                _responseLoginModel.IsNewUser = true;
            }

            // Send email verification link (optional)
            await _unitOfWork.AuthRepository.SendEmailVerificationAsync(user.Email);
            
            // Generate Firebase Token and Access Token
            var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userRecord.Uid);
            var token = _unitOfWork.AuthRepository.GenerateJwtToken(user, "user");
            
            _responseLoginModel.FirebaseToken = firebaseToken;
            _responseLoginModel.AccessToken = token;
            
            return new ResponseModel(
                new ResponseLoginModel(token, firebaseToken, _mapper.Map<UserViewDto>(user), false),
                "Login Successfully", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            // Log the error or handle it
            return new ResponseModel(
                null,
                $"An error occurred during token verification: {ex.Message}", true, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseModel> Register(RequestRegisterModel request)
    {
        try
        {
            if (request.Role == "user")
            {
                // Check if username exists
                var existingUserByUsername = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("username", request.Username);
                if (existingUserByUsername != null)
                {
                    return new ResponseModel(null, "Username is already taken!", false, StatusCodes.Status400BadRequest);
                }

                // Check if email exists
                var existingUserByEmail = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
                if (existingUserByEmail != null)
                {
                    return new ResponseModel(null, "Email is already in use!", false, StatusCodes.Status400BadRequest);
                }

                // Create a new User entity
                var salt = _unitOfWork.AuthRepository.GenerateSalt(); 
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.Password, salt);

                var newUser = new User
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = int.Parse(request.PhoneNumber), // Ensure it's a valid integer
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Status = 1, // Assuming 1 is the status for active users
                    IsTwoFactorEnabled = 0, // Disable 2FA by default
                };

                // Create a Firebase user record
                var userRecordArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    EmailVerified = false,
                    Password = request.Password,
                    DisplayName = request.Username,
                    PhoneNumber = $"+{request.PhoneNumber}", // Ensure the phone number format is correct for Firebase
                    Disabled = false,
                };
                UserRecord firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                newUser.FirebaseUid = firebaseUser.Uid;
                
                // Save the new user to the database
                await _unitOfWork.UserRepo.CreateUser(newUser);

                // Generate a Firebase token for the user using their FirebaseUid
                string firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(newUser.FirebaseUid);

                // Send email verification link
                await _unitOfWork.AuthRepository.SendEmailVerificationAsync(request.Email);
                
                // Map the user to a UserViewDto for the response
                var userViewDto = _mapper.Map<UserViewDto>(newUser);

                // Return success response with Firebase token and user details
                return new ResponseModel(new ResponseRegisterModel(firebaseToken, userViewDto), "User registered successfully! Proceed to verify email.", true, StatusCodes.Status201Created);
            }
            return new ResponseModel(null, "Role is not found!", false, StatusCodes.Status404NotFound);
        } 
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during registration: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseModel> VerifyEmail(RequestVerifyModel request)
    {
        try
        {
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            if (user == null)
            {
                return new ResponseModel(null, "User not found!", false, StatusCodes.Status404NotFound);
            }

            // Check if the code matches the stored code
            /*if (user.TwoFactorSecret == request.VerificationCode)
            {
                // Update the user's email verification status
                user.EmailVerified = true;
                user.IsTwoFactorEnabled = 1; // Enable 2FA after verification
                user.EmailVerificationCode = null; // Clear the verification code
                await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(null, "Email verified successfully! Two-factor authentication is now enabled.", true, StatusCodes.Status200OK);
            }*/

            return new ResponseModel(null, "Invalid verification code!", false, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public Task<ResponseLoginModel> ResetPassword(RequestLoginModel request)
    {
        throw new NotImplementedException();
    }
}