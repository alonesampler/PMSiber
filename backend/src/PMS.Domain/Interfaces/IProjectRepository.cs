using PMS.Domain.Entities;

namespace PMS.Domain.Interfaces;

public interface IProjectRepository
{
    Task CreateAsync(Project entity);
    Task UpdateAsync(Project entity);
    Task DeleteAsync(Project entity);
    public Task<Project[]> GetAllWithFiltersAsync(
        string? name = null,
        string? customerCompanyName = null,
        string? executorCompanyName = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);
    public Task<Project?> GetByIdAsync(Guid id);
}
