using Entity;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using PaymentManagement;
using PaymentManagement.Repository;
using PaymentManagement.Repository.Impl;
using PaymentManagement.Service;
using PaymentManagement.Service.Impl;
using PaymentManagement.Ultility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//db context
builder.Services.AddDbContext<RallyWaveContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("RallyWave"),
        new MySqlServerVersion(new Version(8, 0, 39))); 
});


//service
builder.Services.AddScoped<IPaymentService, PaymentService>();

//repo
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();
builder.Services.AddScoped<ICourtOwnerRepo, CourtOwnerRepo>();
builder.Services.AddScoped<ICourtRepo, CourtRepo>();
builder.Services.AddScoped<IMatchRepo, MatchRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ISubscriptionRepo, SubscriptionRepo>();

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

//utility
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));
builder.Services.AddSingleton(typeof(GetSecret));

//pay-os
var secret = new GetSecret();
var payOsCredentials = secret.GetPayOsCredentials().Result;
var payOs = new PayOS(payOsCredentials!.ClientId, payOsCredentials.ApiKey, payOsCredentials.CheckSumKey);

builder.Services.AddSingleton(payOs);


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
app.UseCors("CORSPolicy");
app.MapControllers();
app.Run();
