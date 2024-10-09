using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public interface IAuthService
{
    public string GenerateJwtToken(User user);
    public Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token);
}
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
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
        catch (InvalidJwtException)
        {
            // Token is invalid or expired
            return null;
        }
    }
}