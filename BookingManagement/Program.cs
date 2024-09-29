using BookingManagement;
using BookingManagement.Service;
using BookingManagement.Service.Impl;
using Entity;
using Microsoft.EntityFrameworkCore;
using BookingManagement.Repository;
using BookingManagement.Repository.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<RallywaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});

//service
builder.Services.AddScoped<IBookingService, BookingService>();


//repo
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();


//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();
