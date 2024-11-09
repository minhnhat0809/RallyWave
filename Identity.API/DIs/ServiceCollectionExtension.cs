using System.Collections;
using System.Configuration;
using Identity.API.Repository;
using Identity.API.Repository.Impl;
using Identity.API.Services;
using Identity.API.Ultility;

namespace Identity.API.DIs;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        // Repositories
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFirebaseStorageRepository, FirebaseStorageRepository>();
        //mapper 
        services.AddAutoMapper(typeof(MapperConfig).Assembly);
        

        //utilities
        services.AddScoped(typeof(Validate));
        services.AddScoped(typeof(ListExtensions));
        
        
        return services;
    }
}