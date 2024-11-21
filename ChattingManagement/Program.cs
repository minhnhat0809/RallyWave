using ChattingManagement;
using ChattingManagement.DIs;
using ChattingManagement.Service.Hubs;
using Entity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//dbcontext
builder.Services.AddDbContext<RallyWaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});

// authentication
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// mapper
builder.Services.AddAutoMapper(typeof(MapperConfig));

// Add Services
builder.Services.AddServices();

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

// signalR
builder.Services.AddSignalR();

var app = builder.Build();
// hubs
app.MapHub<ChatHub>("/chatHub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management");
        c.RoutePrefix = "swagger"; 
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CORSPolicy");
app.MapControllers();
app.Run();
