using Microsoft.EntityFrameworkCore;
using PMS.Domain.Entities;
using PMS.Domain.Interfaces;

namespace PMS.Infrastructure.EfCore.Repositories;

public class ProjectRepository(AppDbContext DbContext) : IProjectRepository
{
    public Task<Project?> GetByIdAsync(Guid id)
        => DbContext.Projects
            .Include(p => p.Manager)
            .Include(p => p.Employees)
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<Project[]> GetAllAsync()
        => DbContext.Projects
            .Include(p => p.Manager)
            .Include(p => p.Employees)
            .ToArrayAsync();

    public Task<Project[]> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null)
    {
        var query = DbContext.Projects
            .Include(p => p.Manager)
            .Include(p => p.Employees)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (!string.IsNullOrWhiteSpace(customerCompanyName))
            query = query.Where(p => p.CustomerCompanyName.Contains(customerCompanyName));

        if (!string.IsNullOrWhiteSpace(executorCompanyName))
            query = query.Where(p => p.ExecutorCompanyName.Contains(executorCompanyName));

        if (startDateFrom.HasValue)
            query = query.Where(p => p.StartDate >= startDateFrom.Value);

        if (startDateTo.HasValue)
            query = query.Where(p => p.StartDate <= startDateTo.Value);

        return query.ToArrayAsync();
    }

    public Task CreateAsync(Project project)
        => DbContext.Projects.AddAsync(project).AsTask();

    public Task UpdateAsync(Project project)
    {
        DbContext.Projects.Update(project);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Project project)
    {
        DbContext.Projects.Remove(project);
        return Task.CompletedTask;
    }
}
