using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Repository.Impl;

public interface IAuthRepository
{
    public string? GenerateJwtToken(User user, string role);
}
public class AuthRepository : IAuthRepository
{
    private readonly IConfiguration _configuration;

    public AuthRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // generate JWT Token 
    public string? GenerateJwtToken(User user, string role)
    {
        if (user.Email != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),             // needed when user register
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, role), // Role come in Token
                new Claim(ClaimTypes.HomePhone, user.PhoneNumber.ToString()),   // needed when court owner register
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
    
    
}