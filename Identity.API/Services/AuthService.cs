using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entity;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public interface IAuthService
{
    public string? GenerateJwtToken(User user);
}
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string? GenerateJwtToken(User user)
    {
        if (user.Email != null)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:Jwt:SecretKey"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Jwt:Issuer"],
                audience: _configuration["Authentication:Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        return null;
    }

}