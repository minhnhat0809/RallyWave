
using System.Text;
using Entity;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.DIs;
using Identity.API.Ultility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var secret = new GetSecret();

var firebaseCredentials = secret.GetFireBaseCredentials().Result;

// Firebase Admin SDK initialization
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseCredentials)
});
// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.WithOrigins() // Adjust the origin to match your frontend
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add Services
builder.Services.AddServices();

//get google credentials
var googleCredentials = secret.GetGoogleCredentials().Result;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(GoogleDefaults.AuthenticationScheme, GoogleDefaults.DisplayName, options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Authority = "https://accounts.google.com";
        options.ClientId = googleCredentials.ClientId;
        options.ClientSecret = googleCredentials.ClientSecret;
        options.ResponseType = "id_token";
        options.CallbackPath = "/google-login";
        options.SaveTokens = true;
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.Scope.Add("openid");
    })
    //
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( builder.Configuration["Authentication:Jwt:SecretKey"] ?? string.Empty)), 
            ValidateIssuer = false, 
            ValidateAudience = false, 
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddAuthorization();


//db context
builder.Services.AddDbContext<RallyWaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});

var app = builder.Build();

// login by google account : FOR TESTING 
app.MapGet("/api/login/google-login", async (HttpContext context) =>
{
    var redirectUrl = context.Request.PathBase + "/api/login/response-token"; 
    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

    // Challenge Google authentication
    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, properties);
});
// response after login
app.MapGet("/api/login/response-token", [Authorize] async (HttpContext context) =>
{
    var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    var testResponse = new TestGoogleLoginModel(false, null, null, null);

    if (result?.Principal == null)
    {
        testResponse.Message = "Unable to authenticate with Google.";
        return Results.BadRequest(testResponse);
    }

    var idToken = result.Properties?.GetTokenValue("id_token");
    
    if (string.IsNullOrEmpty(idToken))
    {
        testResponse.Message = "ID Token is missing.";
        return Results.BadRequest(testResponse);
    }
    testResponse.AccessToken = "accessToken";
    testResponse.IdToken = idToken;
    testResponse.IsSuccess = true;
    testResponse.Message = "Logged in successfully.";
    return Results.Ok(testResponse);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Management");
        c.RoutePrefix = "swagger"; 
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Management");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseCors("CORSPolicy"); 
app.UseAuthentication(); 
app.UseAuthorization(); 


// Map controllers
app.MapControllers();

app.Run();
