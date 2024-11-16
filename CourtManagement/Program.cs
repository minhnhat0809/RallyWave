using Amazon.S3;
using CourtManagement;
using CourtManagement.Repository;
using CourtManagement.Repository.Impl;
using CourtManagement.Service;
using CourtManagement.Service.Impl;
using CourtManagement.Ultility;
using Entity;
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
builder.Services.AddScoped<ICourtService, CourtService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ISlotService, SlotService>();

//repository
builder.Services.AddScoped<ICourtRepo, CourtRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));

//aws
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((_) => true));
});

//mapper 
builder.Services.AddAutoMapper(typeof(MapperConfig).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Management");
        c.RoutePrefix = "swagger"; 
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Management");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CORSPolicy");
app.MapControllers();
app.Run();