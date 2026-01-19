using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using System.Reflection;

namespace PMS.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Project Management System API",
                Version = "v1",
                Description = "Система управления проектами и сотрудниками",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "dev@company.com"
                }
            });

            // add XML comments (if any)
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
            // add JWT Authentication (if needed)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });
        });

        return services;
    }
}
