using Entity;
using MassTransit;
using MatchManagement;
using MatchManagement.Repository;
using MatchManagement.Repository.Impl;
using MatchManagement.Service;
using MatchManagement.Service.Impl;
using MatchManagement.Ultility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//retrieve connection string from AWS Secrets Manager
var getSecret = new GetSecret();
var connectionString = await getSecret.GetConnectionString();

//db context
builder.Services.AddDbContext<RallyWaveContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39))); 
});

//service
builder.Services.AddScoped<IMatchService, MatchService>();

//repo
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFriendShipRepo, FriendShipRepo>();
builder.Services.AddScoped<ISportRepo, SportRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserSportRepo, UserSportRepo>();
builder.Services.AddScoped<IUserMatchRepo, UserMatchRepo>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));

//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder
        .AllowAnyHeader().WithOrigins()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed((_) => true));
});

// mass transit
/*builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest")!);
            host.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest")!);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});*/

var app = builder.Build();

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
