using Entity;
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

//service
builder.Services.AddScoped<IMatchService, MatchService>();

//repo
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));

//db context
builder.Services.AddDbContext<RallywaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});

//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
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
