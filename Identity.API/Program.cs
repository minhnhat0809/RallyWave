using System.Security.Claims;
using Entity;
using Identity.API.DIs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add Services
builder.Services.AddServices();

// Configure Google authentication
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
        options.ResponseType = "id_token"; // Chỉ yêu cầu id_token
        options.CallbackPath = "/google-login"; // Đảm bảo URI này trùng khớp với Google Developer Console
        options.SaveTokens = true; // Lưu access và id tokens
        options.Scope.Add("email");
        options.Scope.Add("profile"); // Nếu bạn cũng muốn thông tin profile
    });



// Configure the database context
builder.Services.AddDbContext<RallywaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39)));
});

var app = builder.Build();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CORSPolicy"); // Apply CORS policy
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization

// Map controllers
app.MapControllers();

app.Run();