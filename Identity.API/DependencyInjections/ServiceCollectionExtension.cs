using System.Collections;
using Identity.API.Services;

namespace Identity.API.DependencyInjections;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // DIs:
        // Services:
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
    
}