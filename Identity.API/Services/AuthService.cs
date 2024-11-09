
using System.Security.Cryptography;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using AutoMapper;
using Entity;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.CourtOwnerModel;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.BusinessObjects.UserViewModel;
using Identity.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Services;

public interface IAuthService
{
    
    public Task<ResponseModel> Login(RequestLoginModel request);
    public Task<ResponseModel> LoginByGoogle(RequestGoogleLoginModel request);
    public Task<ResponseModel> Register(RequestRegisterModel request);
    public Task<ResponseModel> VerifyEmailAccount(RequestVerifyAccountModel requestVerifyDto);
    public Task<ResponseModel> VerifyEmailResetPassword(RequestVerifyModel requestVerifyDto);
    public Task<ResponseModel> ResetPassword(RequestLoginModel request);
    public Task<ResponseModel> ForgetPassword(string email);
    public Task<ResponseModel> ResendVerificationEmailAccount(RequestLoginModel request);
    public Task<ResponseModel> UpdateProfile(ProfileModel request);
    public Task<ResponseModel> ResendVerifyCode(RequestLoginModel request);
    public Task<ResponseModel> UploadUserAvatarAsync(IFormFile avatarFile, string email);
    public Task<ResponseModel> DeleteUserAvatarAsync(string email);
}


public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ResponseLoginModel _responseLoginModel;
    public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _responseLoginModel = new ResponseLoginModel(String.Empty, String.Empty, null, false);
        
    }
    // login by Email Password: User and Court Owner
    public async Task<ResponseModel> Login(RequestLoginModel request)
    {
        try
        {
            // Fetch user from the database
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
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
                            "Invalid password or username", false, StatusCodes.Status404NotFound);
                    }
                }

                // Generate User JWT Token
                var token = _unitOfWork.AuthRepository.GenerateUserJwtToken(user, "user");
                var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.FirebaseUid);
                
                return new ResponseModel(
                    new ResponseLoginModel(token, firebaseToken, _mapper.Map<UserViewDto>(user), false),
                    "Login Successfully", true, StatusCodes.Status200OK);
            }
            // Fetch court owners from the database
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
            if (courtOwner != null)
            {
                // Verify the password
                if (courtOwner is { PasswordSalt: not null, PasswordHash: not null })
                {
                    var hashedPassword =
                        _unitOfWork.AuthRepository.HashPassword(request.Password, courtOwner.PasswordSalt);
                    if (!hashedPassword.SequenceEqual(courtOwner.PasswordHash))
                    {
                        return new ResponseModel(
                            new ResponseLoginModel(String.Empty, String.Empty, null, false),
                            "Invalid password or username", false, StatusCodes.Status404NotFound);
                    }
                }

                // Generate Court Owner JWT Token
                var token = _unitOfWork.AuthRepository.GenerateCourtOwnerJwtToken(courtOwner, "user");
                var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(courtOwner.FirebaseUid);

                return new ResponseModel(
                    new ResponseLoginModel(token, firebaseToken, _mapper.Map<CourtOwnerViewDto>(courtOwner), false),
                    "Login Successfully", true, StatusCodes.Status200OK);
            }
            return new ResponseModel(
                null,
                "Invalid Username or Password", false, StatusCodes.Status404NotFound);
        }
        catch (Exception e)
        {
            return new ResponseModel(
                null,
                $"An error occurred during login: {e.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }
    
    // Login By Google && Register: User and CourtOwner
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
                    "Invalid Google ID token.", false, StatusCodes.Status404NotFound);
            }

            UserRecord userRecord;
            User? user = null;
            CourtOwner? courtOwner = null;
            // Check if user exists in Firebase Authentication
            try
            {
                // Try to get the user by UID (Google UID is used as Firebase UID)
                userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(payload.Subject);
                
                if (request.Role == new Contract().User) user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("firebase-uid", userRecord.Uid);
                if (request.Role == new Contract().CourtOwner) courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("firebase-uid", userRecord.Uid);
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
                
                // Depending on the role, register a new user or court owner
                if (request.Role == new Contract().User)
                {
                    var newUser = new User
                    {
                        UserName = payload.Name,
                        Email = payload.Email,
                        FirebaseUid = payload.Subject, // Store Google UID as Firebase UID
                        Avatar = payload.Picture,
                        PhoneNumber = 0,
                        Gender = "N/A",
                        Dob = DateOnly.FromDateTime(DateTime.Now),
                        Address = "N/A",
                        Province = "N/A",
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        PasswordHash = hashedPassword,
                        PasswordSalt = salt,
                        IsTwoFactorEnabled = 1,
                        TwoFactorSecret = null//_unitOfWork.AuthRepository.GenerateVerificationCode(),
                    };
                    
                    // Generate and send a new verification link
                    string emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(newUser.Email);
                    await _unitOfWork.AuthRepository.SendEmailVerificationAsync(newUser.Email, emailVerificationLink);
                    user = await _unitOfWork.UserRepo.CreateUser(newUser); // Save the user to the database
                    _responseLoginModel.IsNewUser = true;
                }else if (request.Role == new Contract().CourtOwner)
                {
                    var newCourtOwner = new CourtOwner
                    {
                        Name = payload.Name,
                        Email = payload.Email,
                        FirebaseUid = payload.Subject,
                        Avatar = payload.Picture,
                        PhoneNumber = 0,
                        Address = "N/A",
                        Gender = "N/A",
                        Province = "N/A",
                        Status = 1,
                        PasswordHash = hashedPassword,
                        PasswordSalt = salt,
                        IsTwoFactorEnabled = 1,
                        TwoFactorSecret = null //_unitOfWork.AuthRepository.GenerateVerificationCode(),
                    };
                    // Generate and send a new verification link
                    string emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(newCourtOwner.Email);
                    await _unitOfWork.AuthRepository.SendEmailVerificationAsync(newCourtOwner.Email, emailVerificationLink);
                    
                    courtOwner = await _unitOfWork.CourtOwnerRepository.CreateCourtOwner(newCourtOwner); // Save the court owner to the database
                    _responseLoginModel.IsNewUser = true;
                }
            }
            // Generate Firebase Token and Access Token
            var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userRecord.Uid);

            string message = "Login Successfully!";
            if (request.Role == new Contract().User && user != null)
            {
                var token = _unitOfWork.AuthRepository.GenerateUserJwtToken(user, new Contract().User);

                _responseLoginModel.FirebaseToken = firebaseToken;
                _responseLoginModel.AccessToken = token;
                
                // Check if new account
                if (user.IsTwoFactorEnabled == 0)
                {
                    _responseLoginModel.IsNewUser = true;
                    message = "Your verify code had sent to your mail, please check and verify account!";
                }
                
                return new ResponseModel(
                    new ResponseLoginModel(token, firebaseToken, _mapper.Map<UserViewDto>(user), _responseLoginModel.IsNewUser),
                    message, true, StatusCodes.Status200OK);
            }
            if (request.Role == new Contract().CourtOwner && courtOwner != null)
            {
                var token = _unitOfWork.AuthRepository.GenerateCourtOwnerJwtToken(courtOwner, new Contract().CourtOwner);

                // Check if new account
                if (courtOwner.IsTwoFactorEnabled == 0)
                {
                    _responseLoginModel.IsNewUser = true;
                    message = "Your verify code had sent to your mail, please check and verify account!";
                }

                _responseLoginModel.FirebaseToken = firebaseToken;
                _responseLoginModel.AccessToken = token;

                return new ResponseModel(
                    new ResponseLoginModel(token, firebaseToken, _mapper.Map<CourtOwnerViewDto>(courtOwner), _responseLoginModel.IsNewUser),
                    message, true, StatusCodes.Status200OK);
            }

            return new ResponseModel(
                new ResponseLoginModel(string.Empty, string.Empty, null, _responseLoginModel.IsNewUser),
                "Login fail successfully!", false, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            // Log the error or handle it
            return new ResponseModel(
                null,
                $"An error occurred during token verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }
    
    // Register Account: User and Court Owner
    public async Task<ResponseModel> Register(RequestRegisterModel request)
    {
        try
        {
            // USER REGISTER
            if (request.Role == new Contract().User)
            {
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
                    Address = "N/A",
                    Gender = "N/A",
                    Province = "N/A",
                    PhoneNumber = int.Parse(request.PhoneNumber),
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Status = 1, // Active status
                    CreatedDate = DateTime.Now,
                    IsTwoFactorEnabled = 0, // 2FA disabled by default
                    TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode()
                };

                // Create a Firebase user record
                var userRecordArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    EmailVerified = false,
                    Password = request.Password,
                    DisplayName = request.Username,
                    PhoneNumber = $"+84{Int32.Parse(request.PhoneNumber)}", 
                    Disabled = false,
                };
                UserRecord firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                newUser.FirebaseUid = firebaseUser.Uid;

                // Send Verification Email Account
                await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(newUser.Email, newUser.TwoFactorSecret);
                // Save the new user to the database
                await _unitOfWork.UserRepo.CreateUser(newUser);

                // Generate a Firebase custom token for the user
                string firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(newUser.FirebaseUid);
                
                // Map the user to a UserViewDto for the response
                var userViewDto = _mapper.Map<UserViewDto>(newUser);

                // Return success response with Firebase token and user details
                return new ResponseModel(
                    new ResponseRegisterModel(firebaseToken, userViewDto),
                    "User registered successfully! Proceed to verify email.", true, StatusCodes.Status201Created);
            }

            // COURT OWNER REGISTER
            if (request.Role == new Contract().CourtOwner)
            {
                // Check if email exists
                var existingByEmail = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
                if (existingByEmail != null)
                {
                    return new ResponseModel(null, "Email is already in use!", false, StatusCodes.Status400BadRequest);
                }

                // Create a new CourtOwner entity
                var salt = _unitOfWork.AuthRepository.GenerateSalt();
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.Password, salt);

                var courtOwner = new CourtOwner
                {
                    Name = request.Username,
                    Email = request.Email,
                    Address = "N/A",
                    Gender = "N/A",
                    Province = "N/A",
                    PhoneNumber = int.Parse(request.PhoneNumber),
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt,
                    Status = 1, // Active status
                    CreatedDate = DateTime.Now,
                    IsTwoFactorEnabled = 0, 
                    TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode()
                };

                // Create a Firebase user record
                var userRecordArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    EmailVerified = false,
                    Password = request.Password,
                    DisplayName = request.Username,
                    PhoneNumber = $"+84{Int32.Parse(request.PhoneNumber)}", 
                    Disabled = false,
                };
                UserRecord firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                courtOwner.FirebaseUid = firebaseUser.Uid;
                
                // Send Verification Email Account
                await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(courtOwner.Email, courtOwner.TwoFactorSecret);
               
                // Save the new court owner to the database
                await _unitOfWork.CourtOwnerRepository.CreateAsync(courtOwner);

                // Generate a Firebase token for the user
                string firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(courtOwner.FirebaseUid);

                // Send email verification link
                string emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(courtOwner.Email);
                await _unitOfWork.AuthRepository.SendEmailVerificationAsync(courtOwner.Email, emailVerificationLink);

                // Map the court owner to a UserViewDto for the response
                var courtOwnerViewDto = _mapper.Map<CourtOwnerViewDto>(courtOwner);

                // Return success response with Firebase token and user details
                return new ResponseModel(
                    new ResponseRegisterModel(firebaseToken, courtOwnerViewDto),
                    "Court Owner registered successfully! Proceed to verify email.", true, StatusCodes.Status201Created);
            }

            return new ResponseModel(null, "Role is not found!", false, StatusCodes.Status404NotFound);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during registration: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    // Verify Email Account: User and Court Owner
    public async Task<ResponseModel> VerifyEmailAccount(RequestVerifyAccountModel request)
    {
        try
        {
            // CHECK USER FIRST
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            if (user == null)
            {
                // CHECK COURT OWNER LATER
                var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
                if (courtOwner== null) return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                // Check if the code matches the stored code
                if (courtOwner.TwoFactorSecret == request.VerificationCode)
                {
                    // Update the CourtOwner's email verification status
                    courtOwner.IsTwoFactorEnabled = 1; // Enable 2FA after verification
                    courtOwner.TwoFactorSecret = null; // Clear the verification code
                    await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);

                    return new ResponseModel(null, "Email verified successfully! Two-factor authentication is now enabled.", true, StatusCodes.Status200OK);
                }
                return new ResponseModel(null, "Invalid verification code!", false, StatusCodes.Status400BadRequest);
            }
            
            // Check if the code matches the stored code
            if (user.TwoFactorSecret == request.VerificationCode)
            {
                // Update the user's email verification status
                user.IsTwoFactorEnabled = 1; // Enable 2FA after verification
                user.TwoFactorSecret = null; // Clear the verification code
                await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(null, "Email verified successfully! Two-factor authentication is now enabled.", true, StatusCodes.Status200OK);
            }

            return new ResponseModel(null, "Invalid verification code!", false, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }
    
    // Verify Reset Password: User and Court Owner
    public async Task<ResponseModel> VerifyEmailResetPassword(RequestVerifyModel request)
    {
        try
        {
            // CHECK USER FIRST
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            if (user == null)
            {
                // CHECK COURT OWNER LATER
                var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
                if (courtOwner== null) 
                    return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                // Check if the code matches the stored code
                if (courtOwner.TwoFactorSecret == request.VerificationCode)
                {
                    // Create a new CourtOwner entity
                    var salt = _unitOfWork.AuthRepository.GenerateSalt(); 
                    var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.NewPassword, salt);
                
                    // Update the user's password
                    courtOwner.PasswordSalt = salt;
                    courtOwner.PasswordHash = hashedPassword;
                    // Clear the verification code
                    courtOwner.TwoFactorSecret = null; 
                
                    await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);

                    return new ResponseModel(null, "Password being reset successfully!", true, StatusCodes.Status200OK);
                }
                return new ResponseModel(null, "Invalid verification code!", false, StatusCodes.Status400BadRequest);
            }

            // Check if the code matches the stored code
            if (user.TwoFactorSecret == request.VerificationCode)
            {
                // Create a new User entity
                var salt = _unitOfWork.AuthRepository.GenerateSalt(); 
                var hashedPassword = _unitOfWork.AuthRepository.HashPassword(request.NewPassword, salt);
                
                // Update the user's password
                user.PasswordSalt = salt;
                user.PasswordHash = hashedPassword;
                // Clear the verification code
                user.TwoFactorSecret = null; 
                
                await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(null, "Password being reset successfully!", true, StatusCodes.Status200OK);
            }
            return new ResponseModel(null, "Invalid verification code!", false, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }
    
    // Reset Password: User and Court Owner
    public async Task<ResponseModel> ResetPassword(RequestLoginModel request)
    {
        try
        {
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            if (user == null)
            {
                // CHECK COURT OWNER LATER
                var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
                if (courtOwner== null) 
                    return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                // Hash the input password using the same salt
                if (courtOwner.PasswordSalt != null)
                {
                    var hashedPasswordInputCourtOwner = _unitOfWork.AuthRepository.HashPassword(request.Password, courtOwner.PasswordSalt);
                    // Compare the hashed passwords in constant time to avoid timing attacks
                    if (CryptographicOperations.FixedTimeEquals(courtOwner.PasswordHash, hashedPasswordInputCourtOwner))
                    {
                        // Generate verification code
                        courtOwner.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
                        // Save verification code
                        await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);
    
                        // Send email verification code
                        if (courtOwner.Email != null)
                            await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(courtOwner.Email,
                                courtOwner.TwoFactorSecret);

                        return new ResponseModel(null, "Email verification for reset password being sent successfully!", true, StatusCodes.Status200OK);
                    }
                    // If the password does not match
                    return new ResponseModel(null, "Invalid password!", false, StatusCodes.Status400BadRequest);
                }
            }
            else
            {
                if (user.PasswordSalt != null)
                {
                    var hashedPasswordInputUser = _unitOfWork.AuthRepository.HashPassword(request.Password, user.PasswordSalt);
                    // Compare the hashed passwords in constant time to avoid timing attacks
                    if (CryptographicOperations.FixedTimeEquals(user.PasswordHash, hashedPasswordInputUser))
                    {
                        // Generate verification code
                        user.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
                        // Save verification code
                        await _unitOfWork.UserRepo.UpdateUser(user);
    
                        // Send email verification code
                        if (user.Email != null)
                            await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(user.Email, user.TwoFactorSecret);

                        return new ResponseModel(null, "Email verification for reset password being sent successfully!", true, StatusCodes.Status200OK);
                    }
                }
            }

            
            // If the password does not match
            return new ResponseModel(null, "Invalid password!", false, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseModel> ForgetPassword(string email)
    {
        try
        {
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", email);
            if (user == null)
            {
                // CHECK COURT OWNER LATER
                var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", email);
                if (courtOwner== null) 
                    return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                
                // Generate verification code
                courtOwner.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
                // Save verification code
                await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);
    
                // Send email verification code
                if (courtOwner.Email != null)
                    await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(courtOwner.Email,
                        courtOwner.TwoFactorSecret);

                return new ResponseModel(null, "Email verification for password being sent successfully!", true, StatusCodes.Status200OK);

            }
            // Generate verification code
            user.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
            // Save verification code
            await _unitOfWork.UserRepo.UpdateUser(user);
    
            // Send email verification code
            if (user.Email != null)
                await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(user.Email, user.TwoFactorSecret);

            return new ResponseModel(null, "Email verification for reset password being sent successfully!", true, StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    // Resending verification email
    public async Task<ResponseModel> ResendVerificationEmailAccount(RequestLoginModel request)
    {
        string emailVerificationLink;
        // CHECK USER FIRST
        var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
        if (user == null)
        {
            // CHECK COURT OWNER LATER
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
            if (courtOwner== null) 
                return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                
            // Check if the Court Owner has already verified their email
            if (courtOwner.IsTwoFactorEnabled > 0 )
            {
                return new ResponseModel(null, "Email is already verified!", false, StatusCodes.Status400BadRequest);
            }

            // Generate and send a new verification link
            emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(courtOwner.Email);
            if (courtOwner.Email != null)
                await _unitOfWork.AuthRepository.SendEmailVerificationAsync(courtOwner.Email, emailVerificationLink);
            
            return new ResponseModel(null, "Email verification for password being sent successfully!", true, StatusCodes.Status200OK);
        }

        // Check if the user has already verified their email
        if (user.IsTwoFactorEnabled > 0 )
        {
            return new ResponseModel(null, "Email is already verified!", false, StatusCodes.Status400BadRequest);
        }

        // Generate and send a new verification link
        emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(user.Email);
        if (user.Email != null)
            await _unitOfWork.AuthRepository.SendEmailVerificationAsync(user.Email, emailVerificationLink);

        return new ResponseModel(null, "Verification email has been resent. Please check your inbox.", true, StatusCodes.Status200OK);
    }

    public async Task<ResponseModel> UpdateProfile(ProfileModel request)
    {
        try
        {
            // Check if the email is provided
            if (string.IsNullOrEmpty(request.Email))
            {
                return new ResponseModel(null, "Email must be provided.", false, StatusCodes.Status400BadRequest);
            }

            // Try to find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            
            if (user != null)
            {
                // Update Firebase user profile if FirebaseUid is available
                if (!string.IsNullOrEmpty(user.FirebaseUid))
                {
                    var updateRequest = new UserRecordArgs()
                    {
                        Uid = user.FirebaseUid,
                        DisplayName = request.UserName,
                        PhoneNumber = $"+84{request.PhoneNumber}", 
                        Disabled = false,
                        PhotoUrl = request.Avatar,
                    };

                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);
                }

                // User found, update their profile locally
                user.UserName = request.UserName;
                user.PhoneNumber = request.PhoneNumber;
                user.Gender = request.Gender;
                user.Dob = request.Dob;
                user.Address = request.Address;
                user.Province = request.Province;
                user.Avatar = request.Avatar;

                user = await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(user, "User profile updated successfully!", true, StatusCodes.Status200OK);
            }

            // If user is not found, check for court owner
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
            
            if (courtOwner != null)
            {
                // Update Firebase court owner profile if FirebaseUid is available
                if (!string.IsNullOrEmpty(courtOwner.FirebaseUid))
                {
                    var updateRequest = new UserRecordArgs()
                    {
                        Uid = courtOwner.FirebaseUid,
                        DisplayName = request.UserName,
                        PhoneNumber = $"+84{request.PhoneNumber}", 
                        Disabled = false,
                        PhotoUrl = request.Avatar,
                    };

                    await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);
                }

                // Court owner found, update their profile locally
                courtOwner.Name = request.UserName;
                courtOwner.PhoneNumber = request.PhoneNumber;
                courtOwner.Gender = request.Gender;
                courtOwner.Dob = request.Dob;
                courtOwner.Address = request.Address;
                courtOwner.Province = request.Province;
                courtOwner.Avatar = request.Avatar;

                courtOwner = await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);

                return new ResponseModel(courtOwner, "Court owner profile updated successfully!", true, StatusCodes.Status200OK);
            }

            // If neither user nor court owner is found
            return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
        }
        catch (FirebaseAuthException ex)
        {
            return new ResponseModel(null, $"Error updating Firebase user: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred while updating profile: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }


    // Resending Verification Code (SecretVerificationCode)
    public async Task<ResponseModel> ResendVerifyCode(RequestLoginModel request)
    {
        try
        {
            // Find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", request.Email);
            if (user == null)
            {
                // CHECK COURT OWNER LATER
                var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", request.Email);
                if (courtOwner == null) 
                    return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
                
                // Generate verification code
                courtOwner.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
                // Save verification code
                await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);
    
                // Send email verification code
                if (courtOwner.Email != null)
                    await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(courtOwner.Email,
                        courtOwner.TwoFactorSecret);

                return new ResponseModel(null, "Email verification code being sent successfully!", true, StatusCodes.Status200OK);
            }
            // Generate verification code
            user.TwoFactorSecret = _unitOfWork.AuthRepository.GenerateVerificationCode();
    
            // Save verification code
            await _unitOfWork.UserRepo.UpdateUser(user);
    
            // Send email verification code
            if (user.Email != null)
                await _unitOfWork.AuthRepository.SendEmailVerificationCodeAsync(user.Email, user.TwoFactorSecret);

            return new ResponseModel(null, "Email verification for reset password being sent successfully!", true, StatusCodes.Status200OK);

        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred during email verification: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseModel> UploadUserAvatarAsync(IFormFile avatarFile, string email)
    {
        try
        {
            // Check if the email is provided
            if (string.IsNullOrEmpty(email))
            {
                return new ResponseModel(null, "Email must be provided.", false, StatusCodes.Status400BadRequest);
            }

            // Try to find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", email);
            
            if (user != null)
            {
                if (user.Avatar != null)
                {
                    bool isDeleteSucceed = await _unitOfWork.FirebaseStorageRepository.DeleteImageByUrlAsync(user.Avatar);
                    if (!isDeleteSucceed)
                    {
                        return new ResponseModel(null, "Image upload failed in delete old image!", false, StatusCodes.Status500InternalServerError);
                    }
                }

                var url = await _unitOfWork.FirebaseStorageRepository.UploadImageAsync(email, avatarFile, "avatars");
                
                // Update User Avatar
                user.Avatar = url;
                
                user = await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(_mapper.Map<UserViewDto>(user), "User Avatar uploaded successfully!", true, StatusCodes.Status200OK);
            }

            // If user is not found, check for court owner
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", email);
            
            if (courtOwner != null)
            {
                if (courtOwner.Avatar != null)
                {
                    bool isDeleteSucceed = await _unitOfWork.FirebaseStorageRepository.DeleteImageByUrlAsync(user.Avatar);
                    if (!isDeleteSucceed)
                    {
                        return new ResponseModel(null, "Image upload failed in delete old image!", false, StatusCodes.Status500InternalServerError);
                    }
                }

                
                var url = await _unitOfWork.FirebaseStorageRepository.UploadImageAsync(email, avatarFile, "avatars");
                
                // Update User Avatar
                courtOwner.Avatar = url;

                courtOwner = await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);

                return new ResponseModel(_mapper.Map<CourtOwnerViewDto>(courtOwner), "Court Owner Avatar uploaded successfully!", true, StatusCodes.Status200OK);

            }

            // If neither user nor court owner is found
            return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
        }
        catch (FirebaseAuthException ex)
        {
            return new ResponseModel(null, $"Error updating Firebase user: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred while updating profile: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ResponseModel> DeleteUserAvatarAsync(string email)
    {
        try
        {
            // Check if the email is provided
            if (string.IsNullOrEmpty(email))
            {
                return new ResponseModel(null, "Email must be provided.", false, StatusCodes.Status400BadRequest);
            }

            // Try to find the user by email
            var user = await _unitOfWork.UserRepo.GetUserByPropertyAndValue("email", email);
            
            if (user != null)
            {
                if (user.Avatar != null)
                {
                    bool isDeleteSucceed = await _unitOfWork.FirebaseStorageRepository.DeleteImageByUrlAsync(user.Avatar);
                    if (!isDeleteSucceed)
                    {
                        return new ResponseModel(null, "Image upload failed in delete old image!", false, StatusCodes.Status500InternalServerError);
                    }
                }

                user.Avatar = string.Empty;

                user = await _unitOfWork.UserRepo.UpdateUser(user);

                return new ResponseModel(_mapper.Map<UserViewDto>(user), "User Avatar deleted successfully!", true, StatusCodes.Status200OK);
            }

            // If user is not found, check for court owner
            var courtOwner = await _unitOfWork.CourtOwnerRepository.GetCourtOwnerByPropertyAndValue("email", email);
            
            if (courtOwner != null)
            {
                if (courtOwner.Avatar != null)
                {
                    bool isDeleteSucceed = await _unitOfWork.FirebaseStorageRepository.DeleteImageByUrlAsync(user.Avatar);
                    if (!isDeleteSucceed)
                    {
                        return new ResponseModel(null, "Image upload failed in delete old image!", false, StatusCodes.Status500InternalServerError);
                    }
                }

                courtOwner.Avatar = string.Empty;

                courtOwner = await _unitOfWork.CourtOwnerRepository.UpdateCourtOwner(courtOwner);

                return new ResponseModel(_mapper.Map<CourtOwnerViewDto>(courtOwner), "Court Owner Avatar deleted successfully!", true, StatusCodes.Status200OK);

            }

            // If neither user nor court owner is found
            return new ResponseModel(null, "Email not found!", false, StatusCodes.Status404NotFound);
        }
        catch (FirebaseAuthException ex)
        {
            return new ResponseModel(null, $"Error updating Firebase user: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
        catch (Exception ex)
        {
            return new ResponseModel(null, $"An error occurred while updating profile: {ex.Message}", false, StatusCodes.Status500InternalServerError);
        }
    }
}