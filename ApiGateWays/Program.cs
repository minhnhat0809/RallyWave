using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var isDevelopment = builder.Environment.IsDevelopment();
var currentDirectory = Directory.GetCurrentDirectory();
var ocelotConfigFolderPath = Path.Combine(currentDirectory, isDevelopment ? "development" : "production");

try
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    
    /*builder.Configuration.AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.booking.json"), optional: false, 
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.user.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.match.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.chatting.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.notification.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.court.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.identity.json"), optional: false,
        reloadOnChange: true)
        .AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.payment.json"), optional: false,
        reloadOnChange: true);*/
    
    builder.Configuration.AddJsonFile(Path.Combine(ocelotConfigFolderPath, "ocelot.json"), optional: false, 
        reloadOnChange: true);
    
    builder.Services.AddOcelot(builder.Configuration);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}


//cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors", corsPolicyBuilder =>
    {
        corsPolicyBuilder.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("Cors");
app.MapControllers();
app.UseHttpsRedirection();

await app.UseOcelot();


app.Run();
