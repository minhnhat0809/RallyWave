using Entity;
using Microsoft.EntityFrameworkCore;
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

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

//ultility
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));


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
