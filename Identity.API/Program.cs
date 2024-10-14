
using System.Net.Http.Headers;
using System.Text;
using Entity;
using Identity.API.BusinessObjects;
using Identity.API.BusinessObjects.LoginObjects;
using Identity.API.DIs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:7152") // Adjust the origin to match your frontend
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

/*----------------------------------------------------*/
// authen & author
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProject", Version = "v1.0.0" });

    //👇 new code
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
    //👆 new code
});

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
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.ResponseType = "id_token";
        options.CallbackPath = "/google-login";
        options.SaveTokens = true;
        options.Scope.Add("email");
        options.Scope.Add("profile");
        options.Scope.Add("openid");
    })
    
    //AC9d113f318b5c1facf55782e0d1d161fb
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


// Configure the database context
builder.Services.AddDbContext<RallywaveContext>(options =>
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

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CORSPolicy"); 
app.UseAuthentication(); 
app.UseAuthorization(); 


// Map controllers
app.MapControllers();

app.Run();
