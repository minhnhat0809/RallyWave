namespace Identity.API.BusinessObjects;

public class GoogleKeys(string clientId, string clientSecret)
{
    public string ClientId { get; set; } = clientId;
    public string ClientSecret { get; set; } = clientSecret;
}