using Entity;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Identity.API.DIs;
using Microsoft.EntityFrameworkCore;

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
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("rally-wave-firebase.json")
});
// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add Services
builder.Services.AddServices();

builder.Services.AddAuthorization();


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
app.UseCors("CORSPolicy"); 
app.UseAuthentication(); 
app.UseAuthorization(); 


// Map controllers
app.MapControllers();

app.Run();
