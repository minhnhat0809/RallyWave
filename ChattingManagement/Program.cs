using ChattingManagement;
using ChattingManagement.Repository;
using ChattingManagement.Repository.Impl;
using ChattingManagement.Service;
using ChattingManagement.Ultility;
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
builder.Services.AddScoped<IConservationService, ConservationService>();
builder.Services.AddScoped<IMessageService, MessageService>();

//repo
builder.Services.AddScoped<IConservationRepo, ConservationRepo>();
builder.Services.AddScoped<IMessageRepo, MessageRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatting Management");
        c.RoutePrefix = "swagger"; 
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatting Management");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CORSPolicy");
app.MapControllers();
app.Run();
