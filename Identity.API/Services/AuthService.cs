using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Entity;
using Google.Apis.Auth;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.RequestObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UserManagement.DTOs.UserDto;

namespace Identity.API.Services;

public interface IAuthService
{
    public string GenerateJwtToken(User user, GoogleUserProfile googleProfile);
    public Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token);
    public Task<GoogleUserProfile> GetUserInfo(string accessToken);
    public Task<string> ExchangeCodeForAccessToken(string code);
}
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(User user, GoogleUserProfile googleProfile)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", googleProfile.Names?.FirstOrDefault()?.DisplayName ?? user.UserName)
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
    public async Task<GoogleUserProfile> GetUserInfo(string accessToken)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Google People API URL with required fields
            var apiUrl = "https://people.googleapis.com/v1/people/me?personFields=names,emailAddresses,phoneNumbers,addresses";

            try
            {
                var response = await client.GetAsync(apiUrl);
            
                if (!response.IsSuccessStatusCode)
                {
                    // Log the error or throw an exception if needed
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var userProfile = JsonConvert.DeserializeObject<GoogleUserProfile>(content);
            
                return userProfile;
            }
            catch (HttpRequestException e)
            {
                // Handle network issues
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
            catch (JsonException e)
            {
                // Handle JSON deserialization issues
                Console.WriteLine($"JSON error: {e.Message}");
                return null;
            }
        }
    }
    
    public async Task<string> ExchangeCodeForAccessToken(string code)
    {
        using (var client = new HttpClient())
        {
            var values = new Dictionary<string, string?>
            {
                { "code", code },
                { "client_id", _configuration["Authentication:Google:ClientId"] },
                { "client_secret", _configuration["Authentication:Google:ClientSecret"] },
                { "redirect_uri", "your_redirect_uri" },
                { "grant_type", "authorization_code" }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            //var tokenResponse = JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);
            return responseContent; //tokenResponse.AccessToken;
        }
    }


    
}