using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Identity.API.Ultility;

public class GetSecret
{
    public async Task<string> GetFireBaseCredentials()
    {
        const string secretName = "firebase_credentials";
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
}