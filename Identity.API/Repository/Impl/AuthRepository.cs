using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Entity;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Identity.API.Repository.Impl;

public interface IAuthRepository
{
    public byte[] HashPassword(string password, byte[] salt);
    public byte[] GenerateSalt();
    public string GenerateUserJwtToken(User user, string role);
    public string GenerateCourtOwnerJwtToken(CourtOwner courtOwner, string role);
    public Task SendEmailChangeConfirmationAsync(string newEmail);
    public Task SendPasswordResetEmailAsync(string email);
    public Task SendEmailVerificationAsync(string email, string link);
    public Task SendEmailVerificationCodeAsync(string email, string code);
    public string GenerateVerificationCode();
}
public class AuthRepository(IConfiguration configuration) : IAuthRepository 
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
    // generate pass salt
    public byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }
    
    // generate pass hass
    public byte[] HashPassword(string password, byte[] salt)
    {
        var hashedPassword = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

        var pwdString = string.Join(Convert.ToBase64String(salt), Convert.ToBase64String(hashedPassword));
        return Convert.FromBase64String(pwdString);
    }
    
    // generate access token for User
    public string GenerateUserJwtToken(User user, string role)
    {
        try
        {
            if (user.Email != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:SecretKey"] ?? string.Empty));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            throw new Exception("Email address not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
    // generate access token for User
    public string GenerateCourtOwnerJwtToken(CourtOwner courtOwner, string role)
    {
        try
        {
            if (courtOwner.Email != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, courtOwner.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, courtOwner.Name),
                    new Claim(ClaimTypes.Role, role)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Jwt:SecretKey"] ?? string.Empty));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            throw new Exception("Email address not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
    
    // generate verify code
    public string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString(); // Generate a 6-digit code
    }
    
    // email verification link
    public async Task SendEmailVerificationAsync(string email, string link)
    {
        try
        {
            var body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='text-align: center;'>
                            <h2 style='color: #FFA500;'>Welcome to RallyWave!</h2> <!-- Orange color -->
                            <p style='font-size: 16px;'>Please verify your email address by clicking the button below to complete your registration.</p>
                            <a href='{link}' style='display: inline-block; padding: 10px 20px; background-color: #007BFF; color: white; text-decoration: none; border-radius: 5px;'>Verify Email</a> <!-- Blue button -->
                            <p style='font-size: 14px; color: #666;'>If the button doesn't work, you can also click the following link:</p>
                            <a href='{link}' style='font-size: 14px; color: #007BFF;'>{link}</a> <!-- Blue link -->
                            <p style='font-size: 12px; color: #999; margin-top: 20px;'>If you didn't sign up for a RallyWave account, please ignore this email.</p>
                        </div>
                    </body>
                </html>";

            await SendVerificationEmail(email, body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during email verification process: {ex.Message}");
        }
    }
    public async Task SendEmailVerificationCodeAsync(string email, string code)
    {
        try
        {
            var body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='text-align: center;'>
                            <h2 style='color: #FFA500;'>Verify Your Account</h2> <!-- Orange color heading -->
                            <p style='font-size: 16px;'>Please use the 6-digit code below to verify your email address and complete your registration:</p>
                            <div style='margin: 20px 0;'>
                                <span style='font-size: 24px; font-weight: bold; color: #007BFF; padding: 10px; border: 2px dashed #FFA500;'> <!-- Blue code with orange border -->
                                    {code}
                                </span>
                            </div>
                            <p style='font-size: 14px; color: #666;'>If you didn’t request this code, please ignore this email.</p>
                            <p style='font-size: 12px; color: #999; margin-top: 20px;'>Thank you for using RallyWave!</p>
                        </div>
                    </body>
                </html>";

            await SendVerificationEmail(email, body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during email verification process: {ex.Message}");
        }
    }


    private async Task SendVerificationEmail(string email, string body)
    {
        try
        {
            var mailMessage = new MailMessage("info.rallywave@gmail.com", email)
            {
                Subject = "Verify your email address",
                Body = body,
                IsBodyHtml = true,
            };

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("info.rallywave@gmail.com", "njsjphqegtsrdtgu"), // Consider using secure storage for credentials
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Verification email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
    
    // password verification reset
    public async Task SendPasswordResetEmailAsync(string email)
    {
        try
        {
            // Generate the password reset link using Firebase
            var resetLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);

            // Log the reset link (optional for debugging)
            Console.WriteLine($"Password reset link: {resetLink}");

            // Firebase will send the password reset email using its own built-in service
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending the password reset email: {ex.Message}");
        }
    }
    
    // verification change email address
    public async Task SendEmailChangeConfirmationAsync(string newEmail)
    {
        try
        {
            // Send Firebase email change confirmation link
            var link = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(newEmail);
            Console.WriteLine($"Email change confirmation sent to {newEmail}. Link: {link}");
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Error sending email change confirmation: {ex.Message}");
            throw;
        }
    }

    
    // upgrade to enable in firebase
    /*public async Task AddPhoneForTwoFactorAuthenticationAsync(string userId, string phoneNumber)
    {
        try
        {
            // Enroll the user for multi-factor authentication with SMS
            var multiFactorArgs = new MultiFactorInfoArgs
            {
                PhoneInfo = new PhoneInfo
                {
                    PhoneNumber = phoneNumber
                }
            };

            // Add phone number to the user's 2FA methods
            await FirebaseAuth.DefaultInstance.MultiFactor.AddPhoneAsync(userId, multiFactorArgs);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding phone number for 2FA: {ex.Message}");
        }
    }*/
    
    


}