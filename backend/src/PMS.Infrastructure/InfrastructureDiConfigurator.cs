using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Infrastructure.EfCore;

namespace PMS.Infrastructure;

public static class InfrastructureDiConfigurator
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("ConnectionStrings");
        var msSql = section["MsSql"];

        EfCoreDiConfigurator.AddRepositories(services, configuration);
        EfCoreDiConfigurator.ConnectDbContext(services, configuration, msSql);
    }
}
