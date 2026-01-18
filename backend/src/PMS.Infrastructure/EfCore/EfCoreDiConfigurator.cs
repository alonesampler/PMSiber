using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Domain.Interfaces;
using PMS.Infrastructure.EfCore.Repositories;

namespace PMS.Infrastructure.EfCore;

internal static class EfCoreDiConfigurator
{
    internal static void ConnectDbContext(IServiceCollection services, IConfiguration configuration, string connectionString)
    {
         services.AddDbContext<AppDbContext>(options =>
         {
             options.UseNpgsql(connectionString,
                 x =>
                 {
                     x.MigrationsHistoryTable("__EFMigrationsHistory");
                 });
         });
    }
   
    internal static void AddRepositories(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
