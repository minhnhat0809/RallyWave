using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Identity.API.DIs;

public class CustomGoogleHandler : OAuthHandler<GoogleOptions>
{
    [Obsolete("Obsolete")]
    public CustomGoogleHandler(IOptionsMonitor<GoogleOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        var nonce = CodeVerifierGenerator.GenerateNonce(); // Generate a unique nonce
        properties.Items.Add("nonce", nonce); // Add nonce to properties to check later

        var parameters = new Dictionary<string, string>
        {
            { "response_type", "code id_token" },  // Include id_token in response_type
            { "client_id", Options.ClientId },
            { "redirect_uri", redirectUri },
            { "scope", FormatScope() },
            { "state", Options.StateDataFormat.Protect(properties) },
            { "nonce", nonce }  // Add the nonce to the request
        };

        // Handle PKCE if enabled
        if (Options.UsePkce)
        {
            var codeVerifier = CodeVerifierGenerator.GenerateCodeVerifier();
            properties.Items.Add(OAuthConstants.CodeVerifierKey, codeVerifier);
            parameters.Add(OAuthConstants.CodeChallengeKey, CodeVerifierGenerator.GenerateCodeChallenge(codeVerifier));
            parameters.Add(OAuthConstants.CodeChallengeMethodKey, OAuthConstants.CodeChallengeMethodS256);
        }

        return QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters);
    }

}
public abstract class CodeVerifierGenerator
{
    // Generate a code verifier (random string)
    public static string GenerateCodeVerifier()
    {
        var bytes = new byte[32]; // PKCE spec recommends a verifier of length between 43 and 128 characters
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        return Base64UrlEncode(bytes);
    }

    // Generate a code challenge (hashed version of the verifier)
    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            return Base64UrlEncode(bytes);
        }
    }

    // Helper function to Base64 URL encode the verifier or challenge
    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
    public static string GenerateNonce()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
    
        return Base64UrlEncode(bytes);
    }

}