using BookingManagement;
using BookingManagement.Service;
using BookingManagement.Service.Impl;
using Entity;
using Microsoft.EntityFrameworkCore;
using BookingManagement.Repository;
using BookingManagement.Repository.Impl;
using BookingManagement.Ultility;

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
builder.Services.AddScoped<IBookingService, BookingService>();


//repo
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

//ultility
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));
builder.Services.AddScoped(typeof(GetSecret));


//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);


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
