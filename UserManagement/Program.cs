using Entity;
using Microsoft.EntityFrameworkCore;
using UserManagement;
using UserManagement.Repository;
using UserManagement.Repository.Impl;
using UserManagement.Service;
using UserManagement.Service.Impl;
using UserManagement.Ultility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

//service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITeamService, TeamService>();

//repo
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISportRepository, SportRepository>();
builder.Services.AddScoped<IConservationRepository, ConservationRepository>();
builder.Services.AddScoped<IUserTeamRepository, UserTeamRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));
builder.Services.AddHostedService<UserNotificationService>();
//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

//db context
builder.Services.AddDbContext<RallyWaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CORSPolicy");
app.MapControllers();
app.Run();
