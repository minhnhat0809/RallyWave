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
    
    public Task<ResponseLoginModel> Login(RequestLoginModel request);
    public Task<ResponseLoginModel> LoginByGoogle(RequestGoogleLoginModel request);
    public Task<ResponseModel> Register(RequestRegisterModel request);
    public Task<ResponseModel> VerifyEmail(RequestVerifyModel requestVerifyDto);

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
    // login by GOOGLE
    /*public async Task<ResponseGoogleLoginModel> Login(GoogleLoginModel request)
    {
        if (request.IdToken != null)
        {
            var payload = await VerifyGoogleToken(request.IdToken);
            
            // User Role Problem:
            //var userList = await _unitOfWork.UserRepo.GetUsers("email", payload.Email) ;
            var userExist = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email",payload.Email);
            // Not Exist -> add new one
            if (userExist == null) 
            {
                User user = new User()
                {
                    UserName = payload.,
                    Email = payload.Email,
                    PhoneNumber = 0, // Placeholder
                    Gender = "N/A",
                    Dob = new DateOnly(2000, 1, 1),
                    Address = "N/A",
                    Province = "N/A",
                    Avatar = payload.Avatar,
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
    }*/

    public async Task<ResponseLoginModel> Login(RequestLoginModel request)
    {
        try
        {
            // Fetch user from the database
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("username", request.Username);
            if (user != null)
            {
                // Verify the password
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.Password, user.PasswordSalt);
                if (!hashedPassword.SequenceEqual(user.PasswordHash))
                {
                    return new ResponseLoginModel(String.Empty, "Login fail successfully", null, false);
                }

                // Generate JWT Token
                var token = _unitOfWork.AuthRepository.GenerateJwtToken(user, "user");
                // Generate Firebase Token
                var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.FirebaseUid);
                
                return new ResponseLoginModel(token, firebaseToken, _mapper.Map<UserViewDto>(user), false);
            }
            throw new Exception("Invalid username or password.");
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred during login: {e.Message}");
        }
    }

    public async Task<ResponseLoginModel> LoginByGoogle(RequestGoogleLoginModel request)
    {
        try
        {
            // Step 1: Validate the Google ID Token
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
            if (payload == null)
            {
                throw new Exception("Invalid Google ID token.");
            }

            UserRecord userRecord;
            User? user;
            // Step 2: Check if user exists in Firebase Authentication
            try
            {
                // Try to get the user by UID (Google UID is used as Firebase UID)
                userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(payload.Subject);
                user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("firebase-uid", userRecord.Uid);
            }
            catch (FirebaseAuthException)
            {
                // Step 3: If user does not exist, create a new Firebase user
                
                userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
                {
                    Uid = payload.Subject, // Use Google UID as Firebase UID
                    Email = payload.Email,
                    DisplayName = payload.Name,
                    PhotoUrl = payload.Picture
                });

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
                    Status = 1 
                    // Need to set Password default = Abc@Abc1 and auto generate salt to hash 
                };

                user = await _unitOfWork.UserRepo.CreateUser(newUser); // Save user in the database
                _responseLoginModel.IsNewUser = true;
            }

            // Step 4: Send email verification if not verified
            // Send email verification link
            await _unitOfWork.AuthRepository.SendEmailVerificationAsync(user.Email);

            // Step 5: Optionally, handle Two-Factor Authentication setup (SMS or App)
            // Example: Set up SMS-based 2FA
            // await FirebaseAuth.DefaultInstance.MultiFactor.AddPhoneAsync(userRecord.Uid, new MultiFactorInfoArgs { ... });

            // Step 6: Optionally, generate a password reset link
            // var resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(userRecord.Email);
            // await _emailService.SendEmailAsync(userRecord.Email, "Reset your password", $"Please reset your password by clicking on this link: {resetLink}");

            // Step 7: Generate Firebase custom token and Access Token for the user
            var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userRecord.Uid);
            
            var token = _unitOfWork.AuthRepository.GenerateJwtToken(user, "user");
            _responseLoginModel.FirebaseToken = firebaseToken;
            _responseLoginModel.AccessToken = token;
            // Step 8: Return the Firebase custom token
            return _responseLoginModel;
        }
        catch (Exception ex)
        {
            // Log the error or handle it
            throw new Exception($"An error occurred during token verification: {ex.Message}");
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
                var salt = _unitOfWork.AuthRepository.GenerateSalt(); // Assuming you have a utility for generating salt
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.Password, salt);

                var newUser = new User
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = int.Parse(request.PhoneNumber), // Ensure it's a valid integer
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Status = 1, // Assuming 1 is the status for active users
                    /*Role = request.Role, // Assign role*/
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
}