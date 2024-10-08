using Entity;
using Microsoft.EntityFrameworkCore;
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

//service
builder.Services.AddScoped<IUserService, UserService>();

//repo
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//utilities
builder.Services.AddScoped(typeof(Validate));
builder.Services.AddScoped(typeof(ListExtensions));

//cors
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", corsPolicyBuilder => corsPolicyBuilder.AllowAnyHeader().WithOrigins()
        .AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
});

//db context
builder.Services.AddDbContext<RallywaveContext>(options =>
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
