namespace PaymentManagement.DTOs;

public class PayOsDto(string clientId, string apiKey, string checkSumKey)
{
    public string ClientId { get; set; } = clientId;
    public string ApiKey { get; set; } = apiKey;
    public string CheckSumKey { get; set; } = checkSumKey;
}