using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json.Linq;
using PaymentManagement.DTOs;

namespace PaymentManagement.Ultility;

public class GetSecret
{
    public async Task<string> GetConnectionString()
    {
        const string secretName = "connection_string";
        const string region = "ap-southeast-1";

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT", 
        };

        GetSecretValueResponse response;

        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        var secret = response.SecretString;

        return secret;
    }
    
    public async Task<PayOsDto?> GetPayOsCredentials()
    {
        const string secretName = "payosCredentials";
        const string region = "ap-southeast-1";

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        var request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT", 
        };

        GetSecretValueResponse response;

        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        if (response.SecretString == null) return null;
        
        // Assuming your secret is in JSON format
        var secretData = JObject.Parse(response.SecretString);
        var clientId = secretData["ClientId"]?.ToString();
        var apiKey = secretData["ApiKey"]?.ToString();
        var checkSumKey = secretData["CheckSumKey"]?.ToString();

        var payOsDto = new PayOsDto(clientId ?? "", apiKey ?? "", checkSumKey ?? "");

        return payOsDto;
    }
    

}