using System.Collections;
using System.Configuration;
using Identity.API.Services;

namespace Identity.API.DIs;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddSingleton<CustomFirebaseHandler>();
        
        return services;
    }
}