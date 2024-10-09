namespace Identity.API.BusinessObjects;

public class ResponseModel
{
    public Object? User { get; set; }
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }
    public DateTime Expiration { get; set; }  // Expiration time for the tokens
    public string Message { get; set; } = "Login successful";  // Optional message
    public bool IsSuccessful { get; set; } = true;  // Indicates if the operation was successful
    public string? Error { get; set; }  // Error message if there is an issue
}