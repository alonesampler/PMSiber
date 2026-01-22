using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PMS.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Cors");
        var viteFrontend = section["ViteFrontend"];

        services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins(viteFrontend ?? "http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
