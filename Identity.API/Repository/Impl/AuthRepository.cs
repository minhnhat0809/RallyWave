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
    public string GenerateJwtToken(User user, string role);
    public byte[] HashPassword(string password, byte[] salt);
    public byte[] GenerateSalt();
    public string GenerateVerificationCode();
    public Task SendEmailChangeConfirmationAsync(string newEmail);
    public Task SendPasswordResetEmailAsync(string email);
    public Task SendEmailVerificationAsync(string email);
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
    
    // generate access token
    public string GenerateJwtToken(User user, string role)
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
    
    // generate verify code
    public string GenerateVerificationCode()
    {
        Random random = new Random();
        return random.Next(100000, 999999).ToString(); // Generate a 6-digit code
    }
    
    // email verification reset
    public async Task SendEmailVerificationAsync(string email)
    {
        try
        {
            // Generate the email verification link
            string verificationLink = await GenerateEmailVerificationLinkAsync(email);
            Console.WriteLine(verificationLink);
            // Send the verification email with the generated link
            await SendVerificationEmail(email, verificationLink);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during email verification process: {ex.Message}");
        }
    }

    private async Task SendVerificationEmail(string email, string verificationLink)
    {
        try
        {
            var mailMessage = new MailMessage("info.rallywave@gmail.com", email)
            {
                Subject = "Verify your email address",
                //Body = $"Please verify your email by clicking on this link: {verificationLink}",
                Body = "Testing email verification",
                IsBodyHtml = true,
            };

            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587 ,
                Credentials = new NetworkCredential("info.rallywave@gmail.com", "njsjphqegtsrdtgu"),
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
    private async Task<string> GenerateEmailVerificationLinkAsync(string email)
    {
        try
        {
            // Generate the email verification link
            var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(email);
            return emailVerificationLink;
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Error generating email verification link: {ex.Message}");
            throw;
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