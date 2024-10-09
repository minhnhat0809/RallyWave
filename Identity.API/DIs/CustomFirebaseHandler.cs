using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace Identity.API.DIs;

public class FirebaseSettings
{
    public string ApiKey { get; set; }
    public string AuthDomain { get; set; }
    public string ProjectId { get; set; }
    public string StorageBucket { get; set; }
    public string MessagingSenderId { get; set; }
    public string AppId { get; set; }
    public string ServiceAccountKeyPath { get; set; }
}

public class CustomFirebaseHandler
{
    private readonly FirebaseSettings _firebaseSettings;

    public CustomFirebaseHandler(IOptions<FirebaseSettings> firebaseSettings)
    {
        _firebaseSettings = firebaseSettings.Value;

        // Initialize Firebase SDK
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        // Check if FirebaseApp is already initialized to avoid exception
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(_firebaseSettings.ServiceAccountKeyPath)
            });
        }
    }

    public async Task<string> VerifyTokenAsync(string idToken)
    {
        var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        return decodedToken.Uid; // Return the UID or any other relevant information
    }
}